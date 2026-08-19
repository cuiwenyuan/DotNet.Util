using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Util
{
    public partial class JsonUtil
    {
        /// <summary>
        /// �ָ�Json�ַ���Ϊ�ֵ伯�ϡ�
        /// </summary>
        internal class JsonSplit
        {
            internal static bool IsJson(string json)
            {
                int errIndex;
                return IsJson(json, out errIndex);
            }
            internal static bool IsJson(string json, out int errIndex)
            {
                errIndex = 0;
                if (!json.IsNullOrEmpty() && json.Length > 1 &&
                    ((json[0] == '{' && json[json.Length - 1] == '}') || (json[0] == '[' && json[json.Length - 1] == ']')))
                {
                    var cs = new CharState();
                    char c;
                    for (var i = 0; i < json.Length; i++)
                    {
                        c = json[i];
                        if (SetCharState(c, ref cs) && cs.ChildrenStart)//���ùؼ�����״̬��
                        {
                            var item = json.Substring(i);
                            int err;
                            var length = GetValueLength(item, true, out err);
                            cs.ChildrenStart = false;
                            if (err > 0)
                            {
                                errIndex = i + err;
                                return false;
                            }
                            i = i + length - 1;
                        }
                        if (cs.IsError)
                        {
                            errIndex = i;
                            return false;
                        }
                    }

                    return !cs.ArrayStart && !cs.JsonStart;
                }
                return false;
            }
            internal static List<Dictionary<string, string>> Split(string json)
            {
                var result = new List<Dictionary<string, string>>();

                if (!json.IsNullOrEmpty())
                {
                    var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var key = string.Empty;
                    var value = PoolUtil.StringBuilder.Get();
                    var cs = new CharState();
                    try
                    {
                        #region �����߼�
                        char c;
                        for (var i = 0; i < json.Length; i++)
                        {
                            c = json[i];
                            if (!SetCharState(c, ref cs))//���ùؼ�����״̬��
                            {
                                if (cs.JsonStart)//Json�����С�����
                                {
                                    if (cs.KeyStart > 0)
                                    {
                                        key += c;
                                    }
                                    else if (cs.ValueStart > 0)
                                    {
                                        value.Append(c);
                                        //value += c;
                                    }
                                }
                                else if (!cs.ArrayStart)//json�������ֲ������飬���˳���
                                {
                                    break;
                                }
                            }
                            else if (cs.ChildrenStart)//�����ַ���ֵ״̬�¡�
                            {
                                var item = json.Substring(i);
                                int temp;
                                var length = GetValueLength(item, false, out temp);
                                //value = item.Substring(0, length);
                                value.Length = 0;
                                value.Append(item.Substring(0, length));
                                cs.ChildrenStart = false;
                                cs.ValueStart = 0;
                                //cs.state = 0;
                                cs.SetDicValue = true;
                                i = i + length - 1;
                            }
                            if (cs.SetDicValue)//���ü�ֵ�ԡ�
                            {
                                if (!key.IsNullOrEmpty() && !dic.ContainsKey(key))
                                {
                                    //if (value != string.Empty)
                                    //{
                                    //i >= 5 防止越界访问 json[i - 5] 导致 IndexOutOfRangeException
                                    var isNull = i >= 5 && json[i - 5] == ':' && json[i] != '"' && value.Length == 4 && value.ToString() == "null";
                                    if (isNull)
                                    {
                                        value.Length = 0;
                                    }
                                    dic.Add(key, value.ToString());

                                    //}
                                }
                                cs.SetDicValue = false;
                                key = string.Empty;
                                value.Length = 0;
                            }

                            if (!cs.JsonStart && dic.Count > 0)
                            {
                                result.Add(dic);
                                if (cs.ArrayStart)//�������顣
                                {
                                    dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                }
                            }
                        }
                        #endregion
                    }
                    catch
                    {

                    }
                    finally
                    {
                        key = null;
                        value.Length = 0;
                        value.Capacity = 16;
                        value = null;
                    }
                }
                return result;
            }
            /// <summary>
            /// ��ȡֵ�ĳ��ȣ���JsonֵǶ����"{"��"["��ͷʱ��
            /// </summary>
            private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
            {
                errIndex = 0;
                var len = 0;
                if (!json.IsNullOrEmpty())
                {
                    var cs = new CharState();
                    char c;
                    for (var i = 0; i < json.Length; i++)
                    {
                        c = json[i];
                        if (!SetCharState(c, ref cs))//���ùؼ�����״̬��
                        {
                            if (!cs.JsonStart && !cs.ArrayStart)//json�������ֲ������飬���˳���
                            {
                                break;
                            }
                        }
                        else if (cs.ChildrenStart)//�����ַ���ֵ״̬�¡�
                        {
                            var length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//�ݹ���ֵ������һ�����ȡ�����
                            cs.ChildrenStart = false;
                            cs.ValueStart = 0;
                            //cs.state = 0;
                            i = i + length - 1;
                        }
                        if (breakOnErr && cs.IsError)
                        {
                            errIndex = i;
                            return i;
                        }
                        if (!cs.JsonStart && !cs.ArrayStart)//��¼��ǰ����λ�á�
                        {
                            len = i + 1;//���ȱ�����+1
                            break;
                        }
                    }
                }
                return len;
            }
            /// <summary>
            /// �ַ�״̬
            /// </summary>
            private class CharState
            {
                internal bool JsonStart = false;//�� "{"��ʼ��...
                internal bool SetDicValue = false;// ���������ֵ�ֵ�ˡ�
                internal bool EscapeChar = false;//��"\"ת����ſ�ʼ��
                /// <summary>
                /// ���鿪ʼ������һ��ͷ���㡿��ֵǶ�׵��ԡ�childrenStart������ʶ��
                /// </summary>
                internal bool ArrayStart = false;//��"[" ���ſ�ʼ��
                internal bool ChildrenStart = false;//�Ӽ�Ƕ�׿�ʼ�ˡ�
                /// <summary>
                /// ��0 ȡ�����С�����1 ȡֵ�С�
                /// </summary>
                internal int State = -1;

                /// <summary>
                /// ��-1 δ��ʼ������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
                /// </summary>
                internal int KeyStart = -1;
                /// <summary>
                /// ��-1 δ��ʼ������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
                /// </summary>
                internal int ValueStart = -1;
                internal bool IsError = false;//�Ƿ��﷨����

                internal void CheckIsError(char c)//ֻ����һ����������ΪGetLength��ݹ鵽ÿһ���������
                {
                    switch (c)
                    {
                        case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                            IsError = JsonStart && State == 0;//�ظ���ʼ���� ͬʱ����ֵ������
                            break;
                        case '}':
                            IsError = !JsonStart || (KeyStart > -1 && State == 0);//�ظ��������� ���� ��ǰ������
                            break;
                        case '[':
                            IsError = ArrayStart && State == 0;//�ظ���ʼ����
                            break;
                        case ']':
                            IsError = !ArrayStart;//�ظ���ʼ����
                            break;
                        case '"':
                        case '\'':
                            IsError = !JsonStart;//δ��ʼJson
                            break;
                        case ':':
                            IsError = !JsonStart || (JsonStart && KeyStart < 2 && ValueStart < 2 && State == 1);//δ��ʼJson ͬʱ ֻ�ܴ�����ȡֵ֮ǰ��
                            break;
                        case ',':
                            IsError = (!JsonStart && !ArrayStart) || (JsonStart && KeyStart < 2 && ValueStart < 2 && State == 0);//δ��ʼJson ͬʱ ֻ�ܴ�����ȡֵ֮��
                            break;
                        default: //ֵ��ͷ����
                            IsError = !JsonStart || (KeyStart == 0 && ValueStart == 0 && State == 0);//
                            break;
                    }
                    //if (isError)
                    //{

                    //}
                }
            }
            /// <summary>
            /// �����ַ�״̬(����true��Ϊ�ؼ��ʣ�����false��Ϊ��ͨ�ַ�������
            /// </summary>
            private static bool SetCharState(char c, ref CharState cs)
            {
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        #region ������
                        if (cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            cs.CheckIsError(c);
                            if (cs.JsonStart && cs.State == 1)
                            {
                                cs.ValueStart = 0;
                                cs.ChildrenStart = true;
                            }
                            else
                            {
                                cs.State = 0;
                            }
                            cs.JsonStart = true;//��ʼ��
                            return true;
                        }
                        #endregion
                        break;
                    case '}':
                        #region �����Ž���
                        if (cs.KeyStart <= 0 && cs.ValueStart < 2)
                        {
                            cs.CheckIsError(c);
                            if (cs.JsonStart)
                            {
                                cs.JsonStart = false;//����������
                                cs.ValueStart = 0;
                                cs.SetDicValue = true;
                            }
                            return true;
                        }
                        // cs.isError = !cs.jsonStart && cs.state == 0;
                        #endregion
                        break;
                    case '[':
                        #region �����ſ�ʼ
                        if (!cs.JsonStart)
                        {
                            cs.CheckIsError(c);
                            cs.ArrayStart = true;
                            return true;
                        }
                        else if (cs.JsonStart && cs.State == 1 && cs.ValueStart < 2)
                        {
                            cs.CheckIsError(c);
                            //cs.valueStart = 1;
                            cs.ChildrenStart = true;
                            return true;
                        }
                        #endregion
                        break;
                    case ']':
                        #region �����Ž���
                        if (!cs.JsonStart && cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            cs.CheckIsError(c);
                            if (cs.ArrayStart)// && !cs.childrenStart
                            {
                                cs.ArrayStart = false;
                            }
                            return true;
                        }
                        #endregion
                        break;
                    case '"':
                    case '\'':
                        cs.CheckIsError(c);
                        #region ����
                        if (cs.JsonStart)
                        {
                            if (cs.State == 0)//key�׶�
                            {
                                cs.KeyStart = (cs.KeyStart <= 0 ? (c == '"' ? 3 : 2) : 0);
                                return true;
                            }
                            else if (cs.State == 1)//ֵ�׶�
                            {
                                if (cs.ValueStart <= 0)
                                {
                                    cs.ValueStart = (c == '"' ? 3 : 2);
                                    return true;
                                }
                                else if ((cs.ValueStart == 2 && c == '\'') || (cs.ValueStart == 3 && c == '"'))
                                {
                                    if (!cs.EscapeChar)
                                    {
                                        cs.ValueStart = 0;
                                        return true;
                                    }
                                    else
                                    {
                                        cs.EscapeChar = false;
                                    }
                                }

                            }
                        }
                        #endregion
                        break;
                    case ':':
                        cs.CheckIsError(c);
                        #region ð��
                        if (cs.JsonStart && cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 0)
                        {
                            cs.KeyStart = 0;
                            cs.State = 1;
                            return true;
                        }
                        // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                        #endregion
                        break;
                    case ',':
                        cs.CheckIsError(c);
                        #region ����
                        if (cs.JsonStart && cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 1)
                        {
                            cs.State = 0;
                            cs.ValueStart = 0;
                            cs.SetDicValue = true;
                            return true;
                        }
                        else if (cs.ArrayStart && !cs.JsonStart)
                        {
                            return true;
                        }
                        #endregion
                        break;
                    case ' ':
                    case '\r':
                    case '\n':
                        if (cs.JsonStart && cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            return true;//�����ո�
                        }
                        break;
                    default: //ֵ��ͷ����
                        cs.CheckIsError(c);
                        if (c == '\\') //ת�����
                        {
                            if (cs.EscapeChar)
                            {
                                cs.EscapeChar = false;
                            }
                            else
                            {
                                cs.EscapeChar = true;
                                return true;
                            }
                        }
                        else
                        {
                            cs.EscapeChar = false;
                        }
                        if (cs.JsonStart)
                        {
                            if (cs.KeyStart <= 0 && cs.State <= 0)
                            {
                                cs.KeyStart = 1;//�����ŵ�
                            }
                            else if (cs.ValueStart <= 0 && cs.State == 1)
                            {
                                cs.ValueStart = 1;//�����ŵ�
                            }
                        }
                        break;
                }
                return false;
            }


        }
    }
}
