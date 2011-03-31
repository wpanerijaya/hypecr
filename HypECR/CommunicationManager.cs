using System;
using System.Text;
using System.IO.Ports;
using System.Drawing;

namespace HypECR
{
    /// <summary>
    /// class to hold our communication
    /// </summary>
    public class CommunicationManager : SerialPort
    {
        
        #region Manager Enums
        /// <summary>
        /// enumeration to hold our transmission types
        /// </summary>        
        public enum TransmissionType { Text, Hex };

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
        #endregion

        #region Manager Variables
        //property variables
        private TransmissionType _transType;
        //global manager variables
        //private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        #endregion

        #region Manager Properties
        /// <summary>
        /// property to hold our TransmissionType
        /// of our manager class
        /// </summary>
        public TransmissionType CurrentTransmissionType
        {
            get { return _transType; }
            set { _transType = value; }
        }
        #endregion
        
        #region Manager Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="par">Desired Parity</param>
        /// <param name="sBits">Desired StopBits</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="name">Desired PortName</param>
        public CommunicationManager(string baud, string par, string sBits, string dBits, string name)
        {
            this.BaudRate = int.Parse(baud);
            this.Parity = (Parity)Enum.Parse(typeof(Parity), par);
            this.StopBits = (StopBits)Enum.Parse(typeof(StopBits), sBits);
            this.DataBits = int.Parse(dBits);
            this.PortName = name;
        }

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public CommunicationManager()
        {
            _transType = TransmissionType.Hex;
        }
        #endregion      

        #region WriteData
        /// <summary>
        /// method to write data
        /// </summary>
        /// <param name="msg">message to be sent</param>
        /// <returns>message in formatted style</returns>
        public string WriteData(string msg)
        {
            string sMsgReturn;

            switch (CurrentTransmissionType)
            {
                case TransmissionType.Text:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(this.IsOpen == true)) this.Open();
                    //send the message to the port
                    this.Write(msg);
                    //display the message
                    sMsgReturn = msg;
                    break;
                case TransmissionType.Hex:
                    try
                    {
                        //convert the message to byte array
                        byte[] newMsg = DataUtil.HexToByte(msg);
                        //send the message to the port
                        this.Write(newMsg, 0, newMsg.Length);
                        //convert back to hex and display
                        sMsgReturn = DataUtil.ByteToHex(newMsg);
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException("", ex);
                    }
                    break;
                default:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(this.IsOpen == true)) this.Open();
                    //send the message to the port
                    this.Write(msg);
                    //display the message
                    sMsgReturn = msg;
                    break;
            }

            return sMsgReturn;
        }
        #endregion       

        #region OpenPort
        /// <summary>
        /// method to open port
        /// </summary>
        public bool OpenPort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (this.IsOpen == true)
                {
                    this.DiscardInBuffer();
                    this.DiscardOutBuffer();
                    this.Close();
                }
                //now open the port
                this.Open();

                //return Authenticate() + "\n";                
            }
            catch (Exception ex)
            {
                throw new Exception("Error Open Port", ex);
            }
            return true;
        }
        #endregion

        #region ClosePort
        /// <summary>
        /// method to close port
        /// </summary>
        public void ClosePort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (this.IsOpen)
                {
                    this.DiscardInBuffer();
                    this.DiscardOutBuffer();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Close Port", ex);
            }
        }
        #endregion

        #region SetParityValues
        /// <summary>
        /// setter Parity values for combobox
        /// </summary>
        /// <param name="obj">combobox to be set</param>
        public static void SetParityValues(object obj)
        {
            ((System.Windows.Forms.ComboBox)obj).Items.Clear();
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                ((System.Windows.Forms.ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetStopBitValues
        /// <summary>
        /// setter StopBit values for combobox
        /// </summary>
        /// <param name="obj">combobox to be set</param>
        public static void SetStopBitValues(object obj)
        {
            ((System.Windows.Forms.ComboBox)obj).Items.Clear();
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                ((System.Windows.Forms.ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetPortNameValues
        /// <summary>
        /// setter PortName values for combobox
        /// </summary>
        /// <param name="obj">combobox to be set</param>
        public static void SetPortNameValues(object obj)
        {
            ((System.Windows.Forms.ComboBox)obj).Items.Clear();
            foreach (string str in SerialPort.GetPortNames())
            {
                ((System.Windows.Forms.ComboBox)obj).Items.Add(str);
            }
        }
        #endregion
    }
}
