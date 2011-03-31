using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace HypECR
{
    /// <summary>
    /// class for data utilisation
    /// </summary>
    internal class DataUtil
    {
        #region HexToByte
        /// <summary>
        /// method to convert hex string into a byte array
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        public static byte[] HexToByte(string msg)
        {
            try
            {
                //remove any spaces from the string
                msg = msg.Replace(" ", "");
                //create a byte array the length of the
                //divided by 2 (Hex is 2 characters in length)
                byte[] comBuffer = new byte[msg.Length / 2];
                //loop through the length of the provided string
                for (int i = 0; i < msg.Length; i += 2)
                    //convert each set of 2 characters to a byte
                    //and add to the array
                    comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
                //return the array
                return comBuffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// method to convert a byte array into a hex string
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        public static string ByteToHex(byte[] comByte)
        {
            try
            {
                //create a new StringBuilder object
                StringBuilder builder = new StringBuilder(comByte.Length * 3);
                //loop through each byte in the array
                foreach (byte data in comByte)
                    //convert the byte to a string and add to the stringbuilder
                    builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
                //return the converted value
                return builder.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ConvertToHex
        /// <summary>
        /// method to convert ascii string to hex string
        /// </summary>
        /// <param name="asciiString">ascii string to convert</param>
        /// <returns>hex string</returns>
        public static string ConvertToHex(string asciiString)
        {
            try
            {
                string hex = "";
                foreach (char c in asciiString)
                {
                    int tmp = c;
                    hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
                }
                return hex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ConvertToAscii
        /// <summary>
        /// method to convert hex string to ascii string
        /// </summary>
        /// <param name="hexString">hex string to convert</param>
        /// <returns>ascii string</returns>
        public static string ConvertToAscii(string hexString)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i <= hexString.Length - 2; i += 2)
                {
                    char c = Convert.ToChar(Int32.Parse(hexString.Substring(i, 2),
                    System.Globalization.NumberStyles.HexNumber));

                    sb.Append(Char.IsControl(c) ? ' ' : c);

                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ReturnLRC
        /// <summary>
        /// method to return the LRC from the message
        /// </summary>
        /// <param name="pMessage">message to be LRC-ed</param>
        /// <returns>LRC</returns>
        public static char ReturnLRC(string pMessage)
        {            
            try
            {
                int lrcAnswer = 0;
                byte[] byt = HexToByte(pMessage);
                for (int i = 0; i < byt.Length; i++)
                {
                    lrcAnswer = lrcAnswer ^ byt[i];
                }
                return (Char)lrcAnswer;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        #endregion

        #region ConvertToAmountFormat
        /// <summary>
        /// method to convert amount to string formatted
        /// </summary>
        /// <param name="pAmount">amount</param>
        /// <returns>amount string</returns>
        public static string ConvertToAmountFormat(int pAmount)
        {
            return String.Format("{0:000000000000}", pAmount);
        }
        #endregion

        #region ConvertToLengthFormat
        /// <summary>
        /// method to convert length to string formatted
        /// </summary>
        /// <param name="pLength">length</param>
        /// <returns>length string</returns>
        public static string ConvertToLengthFormat(int pLength)
        {
            return String.Format("{0:0000}", pLength);
        }
        #endregion

        /// <summary>
        /// method to encrypt string using key and Triple DES
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pDataToEncrypt"></param>
        /// <returns></returns>
        public static string EncryptStringTripleDES(string pKey, string pDataToEncrypt)
        {                        
            try
            {
                //Console.WriteLine(pKey);
                byte[] TDESKey = HexToByte(pKey);
                // Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Setup the encoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.ANSIX923;

                //Console.WriteLine(pDataToEncrypt);
                byte[] bDataToEncrypt = HexToByte(pDataToEncrypt);

                ICryptoTransform cryptoTransform = TDESAlgorithm.CreateEncryptor();
                byte[] outBlock = cryptoTransform.TransformFinalBlock(bDataToEncrypt, 0, bDataToEncrypt.Length);

                return ByteToHex(outBlock);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        /// <summary>
        /// method to decrypt string using key and Triple DES
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pDataToDecrypt"></param>
        /// <returns></returns>
        public static string DecryptStringTripleDES(string pKey, string pDataToDecrypt)
        {            
            try
            {
                byte[] TDESKey = HexToByte(pKey);
                // Step 2. Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Step 3. Setup the encoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.Zeros;

                byte[] bDataToDecrypt = HexToByte(pDataToDecrypt);
                ICryptoTransform cryptoTransform = TDESAlgorithm.CreateDecryptor();
                byte[] outBlock = cryptoTransform.TransformFinalBlock(bDataToDecrypt, 0, bDataToDecrypt.Length);

                return ByteToHex(outBlock);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
