using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;

namespace HypECR
{
    /// <summary>
    /// class for ECR communication
    /// </summary>
    public class HypECRManager
    {
        #region Manager Variables
        //property variables
        private CommunicationManager.TransmissionType _transType = CommunicationManager.TransmissionType.Hex;
        private string _baudRate;
        private string _parity;
        private string _stopBits;
        private string _dataBits;
        private string _portName;
        private CommunicationManager _commManager = new CommunicationManager();
        private RichTextBox _displayWindow = new RichTextBox();

        //global manager variables        
        private FieldResponse fieldResponse = new FieldResponse();
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private const String MASTER_KEY = "6D58A2BFF10EB34FE6A1F8CDB3C1D5BA";
        private static string SESSION_KEY = String.Empty;
        private bool _isSecureECR = true;
        private bool _isACK = false;
        private bool _isAuthenticate = false;
        private int _timeout = 40;       
        
        //global manager const variables
        /// <summary>
        /// attribute for ACK
        /// </summary>
        private const String ACK = "06";
        /// <summary>
        /// attribute for STX
        /// </summary>
        private const String STX = "02";
        /// <summary>
        /// attribute for ETX
        /// </summary>
        private const String ETX = "03";
        /// <summary>
        /// attribute for Field Separator
        /// </summary>
        private const String FieldSeparator = "1C";
        /// <summary>
        /// value for Transaction Code Sale
        /// </summary>
        private const String TRANSACTION_CODE_SALE = "20";
        /// <summary>
        /// value for Transaction Code Sale Cash
        /// </summary>
        private const String TRANSACTION_CODE_SALECASH = "20";
        /// <summary>
        /// value for Transaction Code Void
        /// </summary>
        private const String TRANSACTION_CODE_VOID = "26";
        /// <summary>
        /// value for Transaction Code Refund
        /// </summary>
        private const String TRANSACTION_CODE_REFUND = "27";
        /// <summary>
        /// value for Transaction Code Settlement
        /// </summary>
        private const String TRANSACTION_CODE_SETTLEMENT = "50";
        /// <summary>
        /// value for Transaction Code OnePurse payment
        /// </summary>
        private const String TRANSACTION_CODE_ONEPURSEPAYMENT = "OP";
        /// <summary>
        /// value for Transaction Code OnePurse Void
        /// </summary>
        private const String TRANSACTION_CODE_ONEPURSEVOID = "OV";
        /// <summary>
        /// value for Transaction Code Comm Test
        /// </summary>
        private const String TRANSACTION_CODE_COMMTEST = "D0";
        #endregion

        #region Manager Properties
        private bool IsSecureECR
        {
            get { return _isSecureECR; }
            set { _isSecureECR = value; }
        }

        private RichTextBox DisplayWindow
        {
            get { return _displayWindow; }
            set { _displayWindow = value; }
        }
       
        /// <summary>
        /// property to hold our TransmissionType
        /// of our manager class
        /// </summary>
        /*public CommunicationManager.TransmissionType CurrentTransmissionType
        {
            get { return _transType; }
            set { _transType = value; }
        }*/

        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        public int TimeOut
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        #endregion

        private class SecureECR
        {
            private string _identifier = "SECR";
            private string _messageType = String.Empty;
            private string _lengthMessage = String.Empty;
            private string _messageData = String.Empty;

            public String Identifier
            {
                get { return _identifier; }
                set { _identifier = value; }
            }

            public String MessageType
            {
                get { return _messageType; }
                set { _messageType = value; }
            }

            public String LengthMessage
            {
                get { return _lengthMessage; }
                set { _lengthMessage = value; }
            }

            public String MessageData
            {
                get { return _messageData; }
                set { _messageData = value; }
            }

            public String GetMessage()
            {
                string sTemp = String.Empty;

                sTemp += DataUtil.ConvertToHex(_identifier + _messageType) + _lengthMessage + _messageData;

                return sTemp;
            }

            /// <summary>
            /// method for parsing Message
            /// </summary>
            public void ParseMessage(string pMessage)
            {
                try
                {
                    pMessage = pMessage.Replace(" ", "");
                    int iPtr = 6;
                    if ((iPtr >= pMessage.Length) && (pMessage.Length < 22))
                        return;

                    _identifier = DataUtil.ConvertToAscii(pMessage.Substring(iPtr, 8));
                    iPtr += 8;
                    _messageType = DataUtil.ConvertToAscii(pMessage.Substring(iPtr, 4));
                    iPtr += 4;
                    _lengthMessage = pMessage.Substring(iPtr, 4);
                    iPtr += 4;
                    _messageData = pMessage.Substring(iPtr, pMessage.Length - iPtr - 4);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            public void PackMessageSend(string pMessageSend)
            {
                try
                {
                    _identifier = "SECR";
                    _messageType = "01";
                    _lengthMessage = DataUtil.ConvertToLengthFormat(pMessageSend.Length / 2);
                    _messageData = DataUtil.EncryptStringTripleDES(SESSION_KEY, pMessageSend).Replace(" ", "");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private class TransportHeader
        {
            private string _transportHeaderType = "60";
            private string _transportDestination = "0000";
            private string _transportSource = "0000";
            private static int _length = 10;

            public String TransportHeaderType
            {
                get { return _transportHeaderType; }
                set { _transportHeaderType = value; }
            }

            public String TransportDestination
            {
                get { return _transportDestination; }
                set { _transportDestination = value; }
            }

            public String TransportSource
            {
                get { return _transportSource; }
                set { _transportSource = value; }
            }

            public static int Length
            {
                get { return _length; }
            }

            public String GetMessage()
            {
                try
                {
                    string sTemp = String.Empty;

                    sTemp += DataUtil.ConvertToHex(_transportHeaderType + _transportDestination + _transportSource);

                    return sTemp;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private class PresentationHeader
        {
            private string _formatVersion = "1";
            private string _requestResponseIndicator = String.Empty;
            private string _transactionCode = String.Empty;
            private string _responseCode = String.Empty;
            private string _moreIndicator = String.Empty;
            private string _fieldSeparator = HypECRManager.FieldSeparator;
            private string _transportHeaderType = String.Empty;
            private string _transportDestination = String.Empty;
            private string _transportSource = String.Empty;

            public static string RequestResponse = "0";
            public static string Response = "1";
            public static string Request = "2";
            public static string LastMessage = "0";
            public static string AnotherMessage = "1";

            public String FormatVersion
            {
                get { return _formatVersion; }
                set { _formatVersion = value; }
            }

            public String RequestResponseIndicator
            {
                get { return _requestResponseIndicator; }
                set { _requestResponseIndicator = value; }
            }

            public String TransactionCode
            {
                get { return _transactionCode; }
                set { _transactionCode = value; }
            }

            public String ResponseCode
            {
                get { return _responseCode; }
                set { _responseCode = value; }
            }

            public String MoreIndicator
            {
                get { return _moreIndicator; }
                set { _moreIndicator = value; }
            }

            public String FieldSeparator
            {
                get { return _fieldSeparator; }
                set { _fieldSeparator = value; }
            }

            public String GetMessage()
            {
                try
                {
                    string sTemp = String.Empty;

                    sTemp += DataUtil.ConvertToHex(_formatVersion + _requestResponseIndicator + _transactionCode + _responseCode + _moreIndicator) + _fieldSeparator;

                    return sTemp;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private class FieldElement
        {
            private string _fieldType = String.Empty;
            private string _fieldLength = String.Empty;
            private string _fieldData = String.Empty;
            private string _fieldSeparator = HypECRManager.FieldSeparator;

            public String FieldType
            {
                get { return _fieldType; }
                set { _fieldType = value; }
            }

            public String FieldLength
            {
                get { return _fieldLength; }
                set { _fieldLength = value; }
            }

            public String FieldData
            {
                get { return _fieldData; }
                set { _fieldData = value; }
            }

            public String FieldSeparator
            {
                get { return _fieldSeparator; }
                set { _fieldSeparator = value; }
            }

            public String GetMessage()
            {
                try
                {
                    string sTemp = String.Empty;

                    sTemp += DataUtil.ConvertToHex(_fieldType) + _fieldLength + DataUtil.ConvertToHex(_fieldData) + _fieldSeparator;

                    return sTemp;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }       

        /// <summary>
        /// default contructor for HypECRManager
        /// </summary>
        public HypECRManager()
        {
            
        }

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           // FieldResponse fr = new FieldResponse();
            try
            {
                //determine the mode the user selected (binary/string)
                switch (_commManager.CurrentTransmissionType)
                {
                    //user chose string
                    case CommunicationManager.TransmissionType.Text:
                        //read data waiting in the buffer
                        string msg = _commManager.ReadExisting();
                        //display the data to the user
                        //DisplayData(CommunicationManager.MessageType.Incoming, msg + "\n");
                        break;
                    //user chose binary
                    case CommunicationManager.TransmissionType.Hex:
                        //retrieve number of bytes in the buffer
                        Thread.Sleep(500);
                        int bytes = _commManager.BytesToRead;

                        if (bytes > 0)
                        {
                            //create a byte array to hold the awaiting data
                            byte[] comBuffer = new byte[bytes];
                            //read the data and store it
                            _commManager.Read(comBuffer, 0, bytes);
                            //mre.Reset();
                            //display the data to the user
                            //DisplayData(CommunicationManager.MessageType.Incoming, DataUtil.ByteToHex(comBuffer) + "\n");

                            if (bytes == 1)
                            {
                                _isACK = WaitACK(comBuffer);
                            }
                            else if (_isACK)
                            {
                                if (_isAuthenticate)
                                {
                                    GetSessionKey(comBuffer);
                                    _isAuthenticate = false;
                                }
                                else
                                {
                                    string sMessage = DataUtil.ByteToHex(comBuffer);
                                    if (sMessage.Length != 0)
                                    {
                                        fieldResponse = ExtractMessage(sMessage);
                                        //DisplayData(CommunicationManager.MessageType.Normal, fieldResponse.ToString());
                                    }
                                }
                                SendACK();
                            }                           
                        }
                        break;
                    default:
                        //read data waiting in the buffer
                        string str = _commManager.ReadExisting();
                        //display the data to the user
                        //DisplayData(CommunicationManager.MessageType.Incoming, str + "\n");
                        break;

                }

            }
            catch (Exception ex)
            {
                //DisplayData(CommunicationManager.MessageType.Error, ex.Message + Environment.NewLine);
                //DisplayData(CommunicationManager.MessageType.Error, ex.StackTrace + Environment.NewLine);
                throw ex;
            }
        }
        #endregion

        private string Authenticate()
        {
            OpenPort();            
            string sMessageToSend = String.Empty;

            SecureECR secureECR = new SecureECR();
            secureECR.MessageType = "00";
            secureECR.LengthMessage = "0016";            
            
            // encrypt zero byte message
            Random rand = new Random();
            int iFirst = rand.Next(0, 255);
            int iLast = rand.Next(0, 255);
            secureECR.MessageData = String.Format( "{0:X2}", iFirst) + "0000000000000000000000000000" + String.Format( "{0:X2}", iLast);
            secureECR.MessageData = DataUtil.EncryptStringTripleDES(MASTER_KEY, secureECR.MessageData).Replace(" ", "");
            
            // generate secure ecr message
            sMessageToSend = secureECR.GetMessage();

            // pack message with standard ecr
            sMessageToSend = PackMessage(sMessageToSend);

            // send it to edc
            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            _isAuthenticate = true;
            _isACK = false;
            GetResponse();
           
            return sMessageToSend;
        }

        /// <summary>
        /// method for sending ACK Command
        /// </summary>
        /// <returns>ACK string</returns>
        private void SendACK()
        {
            string sACK = "06";            
            _commManager.WriteData(sACK);
            //DisplayData(CommunicationManager.MessageType.Outgoing, sACK + Environment.NewLine);
        }

        /// <summary>
        /// method for getting ACK
        /// </summary>
        /// <param name="pACK">byte ACK</param>
        /// <returns>is ACK?</returns>
        private bool WaitACK(byte[] pACK)
        {
            string sACK = "06";
            bool bReturn = false;
            
            if (DataUtil.ByteToHex(pACK).Replace(" ", "").Substring(0, 2).Equals(sACK))
                bReturn = true;

            return bReturn;
        }

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
                if (_commManager.IsOpen == true)
                {
                    _commManager.DiscardInBuffer();
                    _commManager.DiscardOutBuffer();
                    _commManager.Close();
                }
                _commManager.BaudRate = int.Parse(_baudRate);
                _commManager.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);
                _commManager.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);
                _commManager.DataBits = int.Parse(_dataBits);
                _commManager.PortName = _portName;
                _commManager.CurrentTransmissionType = _transType;
                //now open the port
                _commManager.Open();
                int bytes = _commManager.BytesToRead;

                if (bytes > 0)
                {
                    //create a byte array to hold the awaiting data
                    byte[] comBuffer = new byte[bytes];
                    //read the data and store it
                    _commManager.Read(comBuffer, 0, bytes);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region ClosePort
        /// <summary>
        /// method to close port
        /// </summary>
        public bool ClosePort()
        {
            try
            {
                _commManager.ClosePort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion
        

        /// <summary>
        /// method for send sale command
        /// </summary>
        /// <param name="pAmount">amount sale</param>
        /// <returns>sale command string</returns>
        //public string SendSaleCommand(int pAmount)
        public FieldResponse SendSaleCommand(int pAmount)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_SALE;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "40";
            fe.FieldLength = "0012";
            pAmount *= 100;
            fe.FieldData = DataUtil.ConvertToAmountFormat(pAmount);

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();                
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);           
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send void command
        /// </summary>
        /// <param name="pInvoiceNumber">invoice number</param>
        /// <returns>void command string</returns>
        public FieldResponse SendVoidCommand(string pInvoiceNumber)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_VOID;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "65";
            fe.FieldLength = DataUtil.ConvertToLengthFormat(pInvoiceNumber.Length);
            fe.FieldData = pInvoiceNumber;

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send sale cash command
        /// </summary>
        /// <param name="pAmount">sale amount</param>
        /// <param name="pCashBack">cashback amount</param>
        /// <returns>sale cashback string</returns>
        public FieldResponse SendSaleCashCommand(int pAmount, int pCashBack)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_SALECASH;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "40";
            fe.FieldLength = "0012";
            pAmount *= 100;
            fe.FieldData = DataUtil.ConvertToAmountFormat(pAmount);

            sFieldMessage += fe.GetMessage();

            fe.FieldType = "42";
            fe.FieldLength = "0012";
            pCashBack *= 100;
            fe.FieldData = DataUtil.ConvertToAmountFormat(pCashBack);

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send refund command
        /// </summary>
        /// <param name="pAmount">refund amount</param>
        /// <returns>refund command string</returns>
        public FieldResponse SendRefundCommand(int pAmount)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;            

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_REFUND;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "40";
            fe.FieldLength = "0012";
            pAmount *= 100;
            fe.FieldData = DataUtil.ConvertToAmountFormat(pAmount);

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send settle command
        /// </summary>
        /// <param name="pNII">NII</param>
        /// <returns>settle command string</returns>
        public FieldResponse SendSettleCommand(string pNII)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_SETTLEMENT;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "HN";
            fe.FieldLength = DataUtil.ConvertToLengthFormat(pNII.Length);
            fe.FieldData = pNII;

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send OnePurse Payment command
        /// </summary>
        /// <param name="pAmount">payment amount</param>
        /// <returns>OnePurse Payment Command String</returns>
        public FieldResponse SendOnePursePaymentCommand(int pAmount)
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_ONEPURSEPAYMENT;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            fe.FieldType = "40";
            fe.FieldLength = "0012";
            pAmount *= 100;
            fe.FieldData = DataUtil.ConvertToAmountFormat(pAmount);

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method for send OnePurse Void command
        /// </summary>
        /// <returns>OnePurse Void Command String</returns>
        public FieldResponse SendOnePurseVoidCommand()
        {
            fieldResponse = new FieldResponse();
            if (_isSecureECR)
                Authenticate();

            if (_isSecureECR && SESSION_KEY == String.Empty)
                return fieldResponse;

            string sMessageData = String.Empty;
            string sMessageToSend = String.Empty;
            string sFieldMessage = String.Empty;

            TransportHeader th = new TransportHeader();
            PresentationHeader ph = new PresentationHeader();
            FieldElement fe = new FieldElement();

            ph.RequestResponseIndicator = PresentationHeader.RequestResponse;
            ph.TransactionCode = TRANSACTION_CODE_ONEPURSEVOID;
            ph.ResponseCode = "00";
            ph.MoreIndicator = PresentationHeader.LastMessage;

            sFieldMessage += fe.GetMessage();

            sMessageData = th.GetMessage() + ph.GetMessage() + sFieldMessage;

            if (_isSecureECR)
            {
                SecureECR secureECR = new SecureECR();
                secureECR.PackMessageSend(sMessageData);
                sMessageToSend = secureECR.GetMessage();
            }

            sMessageToSend = PackMessage(sMessageToSend);

            _isACK = false;

            Thread.Sleep(2000);

            _commManager.WriteData(sMessageToSend);
            //DisplayData(CommunicationManager.MessageType.Outgoing, _commManager.WriteData(sMessageToSend) + "\n");

            GetResponse();
            return fieldResponse;
        }

        /// <summary>
        /// method to pack message for hypercom ECR format
        /// </summary>
        /// <param name="pMessageData">message to be packed</param>
        /// <returns>packed message</returns>
        private string PackMessage(string pMessageData)
        {
            string sPackMessage = String.Empty;

            sPackMessage = String.Format("{0:0000}", pMessageData.Length / 2) + pMessageData + ETX;

            sPackMessage = STX + sPackMessage + DataUtil.ConvertToHex(DataUtil.ReturnLRC(sPackMessage).ToString());

            return sPackMessage;
        }

        /// <summary>
        /// method to extract message and put it in FieldResponse
        /// </summary>
        /// <param name="pMessage">message to be extracted</param>
        /// <returns>field response</returns>
        private FieldResponse ExtractMessage(string pMessage)
        {
            int iPtr = 6;
            SecureECR secureECR = new SecureECR();
            FieldResponse fr = new FieldResponse();
            string sMessage = pMessage.Replace(" ", "");
            if (_isSecureECR)
            {
                secureECR.ParseMessage(sMessage);
                sMessage = DataUtil.DecryptStringTripleDES(SESSION_KEY, secureECR.MessageData);
                iPtr = 0;
            }
            try
            {
                sMessage = sMessage.Replace(" ", "");
                if ((iPtr > sMessage.Length) && (sMessage.Length < TransportHeader.Length * 2 + 12))
                    return fr;
                string sTransportHeader = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, TransportHeader.Length * 2));
                iPtr += TransportHeader.Length * 2;
                if (!sTransportHeader.Equals("6000000000"))
                    fr.ResponseCode = "XX";

                PresentationHeader ph = new PresentationHeader();
                ph.FormatVersion = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, 2));
                iPtr += 2;
                ph.RequestResponseIndicator = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, 2));
                iPtr += 2;
                ph.TransactionCode = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, 4));
                iPtr += 4;
                ph.ResponseCode = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, 4));
                iPtr += 4;
                ph.MoreIndicator = DataUtil.ConvertToAscii(sMessage.Substring(iPtr, 2));
                iPtr += 2;
                ph.FieldSeparator = sMessage.Substring(iPtr, 2);
                iPtr += 2;

                int iLength = sMessage.Length - iPtr - 4;
                if (_isSecureECR)
                    iLength = int.Parse(secureECR.LengthMessage) * 2 - iPtr;
                fr.TransactionCode = ph.TransactionCode;
                fr.ResponseCode = ph.ResponseCode;
                if (iLength > sMessage.Length)
                {
                    fr.ResponseCode = "FE";
                    return fr;
                }
                fr.ParseMessage(sMessage.Substring(iPtr, iLength));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fr;
        }

        private void GetSessionKey(byte[] pSessionKey)
        {
            try
            {
                string sMessage = DataUtil.ByteToHex(pSessionKey).Replace(" ","");
                SecureECR secureECR = new SecureECR();               
                secureECR.ParseMessage(sMessage);
                if (secureECR.Identifier != "SECR" && secureECR.MessageType != "00" && secureECR.LengthMessage != "0016")
                    throw new Exception("Invalid Authentication");

                SESSION_KEY = DataUtil.DecryptStringTripleDES(MASTER_KEY, secureECR.MessageData).Replace(" ", "").Substring(0, 32);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool GetResponse()
        {
            try
            {    
                Thread.Sleep(500);
                int bytes = 0;
                bool bTimeOut = false;
                while (true)
                {
                    bytes = _commManager.BytesToRead;
                    if (bytes > 0)
                    {
                        //create a byte array to hold the awaiting data
                        byte[] comBuffer = new byte[1037];
                        //read the data and store it
                        _commManager.Read(comBuffer, 0, 1037);
                        //display the data to the user
                        //DisplayData(CommunicationManager.MessageType.Incoming, DataUtil.ByteToHex(comBuffer) + "\n");

                        if (bytes == 1)
                        {
                            _isACK = WaitACK(comBuffer);
                            if (_isACK)
                                bTimeOut = false;                            
                        }
                        else if (_isACK)
                        {
                            SendACK();
                            if (_isAuthenticate)
                            {
                                GetSessionKey(comBuffer);
                                _isAuthenticate = false;
                                return true;
                            }
                            else
                            {
                                string sMessage = DataUtil.ByteToHex(comBuffer);
                                if (sMessage.Length != 0)
                                {
                                    fieldResponse = ExtractMessage(sMessage);
                                    //DisplayData(CommunicationManager.MessageType.Normal, fieldResponse.ToString());
                                }
                                return true;
                            }
                        }
                    }
                    if (bTimeOut)
                    {
                        fieldResponse.ResponseCode = "TO";
                        return !bTimeOut;
                    }

                    int i = 0;

                    bytes = _commManager.BytesToRead;

                    while (bytes == 0)
                    {
                        bytes = _commManager.BytesToRead;
                        if (i < (1000 * _timeout))
                        {
                            Thread.Sleep(1);
                            i++;
                        }
                        else
                        {
                            fieldResponse.ResponseCode = "TO";
                            return false;
                        }
                    }

                    if (_isAuthenticate)
                    {
                        Thread.Sleep(1000);
                        bTimeOut = true;
                    }
                    else if (_isACK)
                    {
                        Thread.Sleep(1000);
                        bTimeOut = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        bTimeOut = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                //DisplayData(CommunicationManager.MessageType.Error, ex.Message + Environment.NewLine);
                //DisplayData(CommunicationManager.MessageType.Error, ex.StackTrace + Environment.NewLine);
                throw ex;
            }            
        }
     
        #region DisplayData
        /// <summary>
        /// method to display the data to and from the port
        /// on the screen
        /// </summary>
        /// <param name="type">MessageType of the message</param>
        /// <param name="msg">Message to display</param>
        [STAThread]
        private void DisplayData(CommunicationManager.MessageType type, string msg)
        {
            _displayWindow.Invoke(new EventHandler(delegate
            {
                _displayWindow.SelectedText = string.Empty;
                _displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Bold);
                _displayWindow.SelectionColor = MessageColor[(int)type];
                _displayWindow.AppendText(msg);
                _displayWindow.ScrollToCaret();
            }));
        }
        #endregion        
    }
}
