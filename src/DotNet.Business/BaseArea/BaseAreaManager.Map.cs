//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using DotNet.Util;
using Newtonsoft.Json;

namespace DotNet.Business
{

    /// <summary>
    /// BaseAreaManager 
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2014.02.11 版本：1.0 JiRiGaLa  表中添加是否可删除，可修改字段。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014.02.11</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaManager : BaseManager
    {
        #region BaiduGeocodingByAddress(out string result, string province = null, string city = null)更新百度坐标 宋彪
        /// <summary>
        /// 更新百度坐标 宋彪
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isAll">是否更新已存在座标</param>
        /// <returns></returns>
        public bool BaiduGeocodingByAddress(out string result, bool isAll)
        {
            // 遍历全部城市表
            var dt = GetDataTable();
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            StreamReader str = null;
            dynamic parsedObject = null;
            string jsonStr = string.Empty, lng = string.Empty, lat = string.Empty;
            result = string.Empty;
            string tmpProvince = string.Empty, tmpCity = string.Empty, tmpFullname = string.Empty;
            string tmpLng = string.Empty, tmpLat = string.Empty, tmpId = string.Empty;
            var reqStr = string.Empty;
            var sb = Pool.StringBuilder.Get();
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        tmpLng = dt.Rows[i]["LONGITUDE"].ToString();
                        tmpLat = dt.Rows[i]["LATITUDE"].ToString();
                        //isALL=false或者tmpLng不为空或者tmpLat不为空 则不更新已存在座标
                        if (isAll)
                        {
                            if (!string.IsNullOrWhiteSpace(tmpLng) && !string.IsNullOrWhiteSpace(tmpLat))
                            {
                                UpdateAreaCoordinate(dt, ref req, ref res, ref str, ref parsedObject, ref jsonStr, ref lng, ref lat, ref tmpProvince, ref tmpCity, ref tmpFullname, ref tmpId, ref reqStr, sb, i);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(tmpLng) || string.IsNullOrWhiteSpace(tmpLat))
                            {
                                UpdateAreaCoordinate(dt, ref req, ref res, ref str, ref parsedObject, ref jsonStr, ref lng, ref lat, ref tmpProvince, ref tmpCity, ref tmpFullname, ref tmpId, ref reqStr, sb, i);
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        result = sb.Append("全部坐标转换成功，本次转换" + dt.Rows.Count + "个坐标信息").Put();
                    }
                    else
                    {
                        result = "本次转换，部分坐标转换不成功：" + sb.Put();
                    }
                    return true;
                }
                else
                {
                    result = "没有指定条件的地区，本次没有进行任何转换！";
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = "坐标转换失败：" + ex;
                return false;
            }
        }

        private void UpdateAreaCoordinate(DataTable dt, ref HttpWebRequest req, ref HttpWebResponse res, ref StreamReader str, ref dynamic parsedObject, ref string jsonStr, ref string lng, ref string lat, ref string tmpProvince, ref string tmpCity, ref string tmpFullname, ref string tmpId, ref string reqStr, StringBuilder sb, int i)
        {
            tmpProvince = dt.Rows[i]["PROVINCE"].ToString();
            tmpCity = dt.Rows[i]["CITY"].ToString();
            tmpFullname = dt.Rows[i]["FULLNAME"].ToString();
            tmpId = dt.Rows[i]["ID"].ToString();
            reqStr = tmpProvince + tmpCity + tmpFullname;
            if (!string.IsNullOrWhiteSpace(reqStr))
            {
                req = (HttpWebRequest)WebRequest.Create("http://api.map.baidu.com/geocoder/v2/?output=json&ak=92edfa41713390920bc55acb369e9447&address=" + reqStr);
                res = (HttpWebResponse)req.GetResponse();
                str = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                jsonStr = str.ReadToEnd();
                parsedObject = JsonConvert.DeserializeObject(jsonStr);
                if (parsedObject["status"] == 0)
                {
                    lng = parsedObject["result"]["location"]["lng"];
                    lat = parsedObject["result"]["location"]["lat"];
                    //更新方法
                    UpdateAreaCoordinate(tmpId, lng, lat);
                }
                else
                {
                    sb.Append(tmpId + ":" + tmpProvince + tmpCity + tmpFullname + "解析不成功，");
                }
            }
            else
            {
                sb.Append(tmpId + ":" + tmpProvince + tmpCity + tmpFullname + "省市县均为空，解析不成功！");
            }
        }
        #endregion

        #region public int UpdateAreaCoordinate(int id, string lng, string lat)
        /// <summary>
        /// 根据id、经度、纬度更新百度坐标 宋彪 2014.02.28
        /// 对外可以通过调用这个方法实现单个地理位置座标的更新
        /// </summary>
        /// <param name="lng">百度经度</param>
        /// <param name="lat">百度纬度</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateAreaCoordinate(string id, string lng, string lat)
        {
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(lng) && !string.IsNullOrWhiteSpace(lat))
            {
                var sb = Pool.StringBuilder.Get();
                sb.Append("UPDATE BaseArea SET ");
                IDbDataParameter[] dbParameters = null;
                sb.Append(" Longitude   = " + dbHelper.GetParameter("lng"));
                sb.Append(" ,Latitude = " + dbHelper.GetParameter("lat"));
                sb.Append("  WHERE ID = " + dbHelper.GetParameter("id"));
                dbParameters = new IDbDataParameter[] { 
                    dbHelper.MakeParameter("lng", lng), 
                    dbHelper.MakeParameter("lat", lat), 
                    dbHelper.MakeParameter("id", id)
                };
                return dbHelper.ExecuteNonQuery(sb.Put(), dbParameters);
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}