using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenshiModifier
{
    class ValueHelper
    {
        /// <summary>
        /// 将二进制值转ASCII格式十六进制字符串
        /// </summary>
        /// <paramname="data">二进制值</param>
        /// <paramname="length">定长度的二进制</param>
        /// <returns>ASCII格式十六进制字符串</returns>
        public static string toHexString(int data, int length)
        {
            string result = "";
            if (data > 0)
                result = Convert.ToString(data, 16).ToUpper();
            if (result.Length < length)
            {
                // 位数不够补0
                StringBuilder msg = new StringBuilder(0);
                msg.Length = 0;
                msg.Append(result);
                for (; msg.Length < length; msg.Insert(0, "0")) ;
                result = msg.ToString();
            }
            return result;
        }

        ///<summary>
        /// 将浮点数转ASCII格式十六进制字符串（符合IEEE-754标准（32））
        /// </summary>
        /// <paramname="data">浮点数值</param>
        /// <returns>十六进制字符串</returns>
        public static string floatToIntString(float data)
        {
            byte[] intBuffer = BitConverter.GetBytes(data);
            StringBuilder stringBuffer = new StringBuilder(0);
            for (int i = 0; i < intBuffer.Length; i++)
            {
                stringBuffer.Append(toHexString(intBuffer[i] & 0xff, 2));
            }
            return stringBuffer.ToString();
        }

        ///<summary>
        /// 将ASCII格式十六进制字符串转浮点数（符合IEEE-754标准（32））
        /// </summary>
        /// <paramname="data">十六进制字符串</param>
        /// <returns>浮点数值</returns>
        public static float intStringToFloat(String data)
        {
            if (data.Length < 8 || data.Length > 8)
            {
                //throw new NotEnoughDataInBufferException(data.length(), 8);
                throw (new ApplicationException("缓存中的数据不完整。"));
            }
            else
            {
                byte[] intBuffer = new byte[4];
                // 将16进制串按字节逆序化（一个字节2个ASCII码）
                for (int i = 0; i < 4; i++)
                {
                    intBuffer[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
                }
                return BitConverter.ToSingle(intBuffer, 0);
            }
        }
    }
}
