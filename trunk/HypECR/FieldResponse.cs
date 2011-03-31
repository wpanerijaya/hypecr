using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HypECR
{
    /// <summary>
    /// class for Field Response
    /// </summary>
    public class FieldResponse
    {
        private string _approvalCode = String.Empty;
        private string _responseText = String.Empty;
        private string _transactionDate = String.Empty;
        private string _transactionTime = String.Empty;
        private string _terminalID = String.Empty;
        private string _cardNumber = String.Empty;
        private string _expiryDate = String.Empty;
        private string _amountTransaction = String.Empty;
        private string _amountTip = String.Empty;
        private string _batchNumber = String.Empty;
        private string _batchAmount = String.Empty;
        private string _invoiceNumber = String.Empty;
        private string _merchantNameAndAddress = String.Empty;
        private string _merchantNumber = String.Empty;
        private string _cardIssuerName = String.Empty;
        private string _retrievalReferenceNumber = String.Empty;
        private string _cardIssuerID = String.Empty;
        private string _cardHolderName = String.Empty;
        private string _systemTraceNo = String.Empty;
        private string _batchTotal = String.Empty;
        private string _nii = String.Empty;
        private string _transactionCode = String.Empty;
        private string _responseCode = String.Empty;

        /// <summary>
        /// method for calling ApprovalCode
        /// </summary>
        public String ApprovalCode
        {
            get { return _approvalCode; }
            set { _approvalCode = value; }
        }

        /// <summary>
        /// method for calling ResponseText
        /// </summary>
        public String ResponseText
        {
            get { return _responseText; }
            set { _responseText = value; }
        }

        /// <summary>
        /// method for calling TransactionDate
        /// </summary>
        public String TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; }
        }

        /// <summary>
        /// method for calling TransactionTime
        /// </summary>
        public String TransactionTime
        {
            get { return _transactionTime; }
            set { _transactionTime = value; }
        }

        /// <summary>
        /// method for calling TerminalID
        /// </summary>
        public String TerminalID
        {
            get { return _terminalID; }
            set { _terminalID = value; }
        }

        /// <summary>
        /// method for calling CardNumber
        /// </summary>
        public String CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; }
        }

        /// <summary>
        /// method for calling ExpiryDate
        /// </summary>
        public String ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value; }
        }

        /// <summary>
        /// method for calling AmountTransaction
        /// </summary>
        public String AmountTransaction
        {
            get { return _amountTransaction; }
            set { _amountTransaction = value; }
        }

        /// <summary>
        /// method for calling AmountTip
        /// </summary>
        public String AmountTip
        {
            get { return _amountTip; }
            set { _amountTip = value; }
        }

        /// <summary>
        /// method for calling BatchNumber
        /// </summary>
        public String BatchNumber
        {
            get { return _batchNumber; }
            set { _batchNumber = value; }
        }

        /// <summary>
        /// method for calling BatchAmount
        /// </summary>
        public String BatchAmount
        {
            get { return _batchAmount; }
            set { _batchAmount = value; }
        }

        /// <summary>
        /// method for calling InvoiceNumber
        /// </summary>
        public String InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { _invoiceNumber = value; }
        }

        /// <summary>
        /// method for calling MerchantNameAndAddress
        /// </summary>
        public String MerchantNameAndAddress
        {
            get { return _merchantNameAndAddress; }
            set { _merchantNameAndAddress = value; }
        }

        /// <summary>
        /// method for calling MerchantNumber
        /// </summary>
        public String MerchantNumber
        {
            get { return _merchantNumber; }
            set { _merchantNumber = value; }
        }

        /// <summary>
        /// method for calling CardIssuerName
        /// </summary>
        public String CardIssuerName
        {
            get { return _cardIssuerName; }
            set { _cardIssuerName = value; }
        }

        /// <summary>
        /// method for calling RetrievalReferenceNumber
        /// </summary>
        public String RetrievalReferenceNumber
        {
            get { return _retrievalReferenceNumber; }
            set { _retrievalReferenceNumber = value; }
        }

        /// <summary>
        /// method for calling CardIssuerID
        /// </summary>
        public String CardIssuerID
        {
            get { return _cardIssuerID; }
            set { _cardIssuerID = value; }
        }

        /// <summary>
        /// method for calling CardHolderName
        /// </summary>
        public String CardHolderName
        {
            get { return _cardHolderName; }
            set { _cardHolderName = value; }
        }

        /// <summary>
        /// method for calling SystemTraceNo
        /// </summary>
        public String SystemTraceNo
        {
            get { return _systemTraceNo; }
            set { _systemTraceNo = value; }
        }

        /// <summary>
        /// method for calling BatchTotal
        /// </summary>
        public String BatchTotal
        {
            get { return _batchTotal; }
            set { _batchTotal = value; }
        }

        /// <summary>
        /// method for calling NII
        /// </summary>
        public String NII
        {
            get { return _nii; }
            set { _nii = value; }
        }

        /// <summary>
        /// method for calling TransactionCode
        /// </summary>
        public String TransactionCode
        {
            get { return _transactionCode; }
            set { _transactionCode = value; }
        }

        /// <summary>
        /// method for calling ResponseCode
        /// </summary>
        public String ResponseCode
        {
            get { return _responseCode; }
            set { _responseCode = value; }
        }

        /// <summary>
        /// method for parsing Message
        /// </summary>
        internal void ParseMessage(string pFieldData)
        {
            try
            {
                int iPtr = 0;
                do
                {
                    if (iPtr >= pFieldData.Length && pFieldData.Length < 8)
                        break;
                    string sFieldType = DataUtil.ConvertToAscii(pFieldData.Substring(iPtr, 4));
                    iPtr += 4;
                    int iLength = int.Parse(pFieldData.Substring(iPtr, 4));
                    iPtr += 4;
                    string sFieldData = DataUtil.ConvertToAscii(pFieldData.Substring(iPtr, iLength * 2));
                    iPtr += iLength * 2;
                    if (sFieldType.Equals("01"))
                    {
                        this._approvalCode = sFieldData;
                    }
                    else if (sFieldType.Equals("02"))
                    {
                        this._responseText = sFieldData;
                    }
                    else if (sFieldType.Equals("03"))
                    {
                        this._transactionDate = sFieldData;
                    }
                    else if (sFieldType.Equals("04"))
                    {
                        this._transactionTime = sFieldData;
                    }
                    else if (sFieldType.Equals("16"))
                    {
                        this._terminalID = sFieldData;
                    }
                    else if (sFieldType.Equals("30"))
                    {
                        this._cardNumber = sFieldData;
                    }
                    else if (sFieldType.Equals("31"))
                    {
                        this._expiryDate = sFieldData;
                    }
                    else if (sFieldType.Equals("40"))
                    {
                        float fAmount = float.Parse(sFieldData) / 100;
                        this._amountTransaction = fAmount.ToString();
                    }
                    else if (sFieldType.Equals("41"))
                    {
                        float fAmount = float.Parse(sFieldData) / 100;
                        this._amountTip = fAmount.ToString();
                    }
                    else if (sFieldType.Equals("50"))
                    {
                        this._batchNumber = sFieldData;
                    }
                    else if (sFieldType.Equals("52"))
                    {
                        float fAmount = float.Parse(sFieldData) / 100;
                        this._batchAmount = fAmount.ToString();
                    }
                    else if (sFieldType.Equals("65"))
                    {
                        this._invoiceNumber = sFieldData;
                    }
                    else if (sFieldType.Equals("D0"))
                    {
                        this._merchantNameAndAddress = sFieldData;
                    }
                    else if (sFieldType.Equals("D1"))
                    {
                        this._merchantNumber = sFieldData;
                    }
                    else if (sFieldType.Equals("D2"))
                    {
                        this._cardIssuerName = sFieldData;
                    }
                    else if (sFieldType.Equals("D3"))
                    {
                        this._retrievalReferenceNumber = sFieldData;
                    }
                    else if (sFieldType.Equals("D4"))
                    {
                        this._cardIssuerID = sFieldData;
                    }
                    else if (sFieldType.Equals("D5"))
                    {
                        this._cardHolderName = sFieldData;
                    }
                    else if (sFieldType.Equals("D9"))
                    {
                        this._systemTraceNo = sFieldData;
                    }
                    else if (sFieldType.Equals("HO"))
                    {
                        this._batchTotal = sFieldData;
                    }
                    else if (sFieldType.Equals("HN"))
                    {
                        this._nii = sFieldData;
                    }

                    if (pFieldData.Substring(iPtr, 2).Equals("1C"))

                        iPtr += 2;

                } while (iPtr < pFieldData.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Transaction Code : " + _transactionCode + "\n");
            sb.Append("Response Code : " + _responseCode + "\n");
            sb.Append("Approval Code : " + _approvalCode + "\n");
            sb.Append("Response Text : " + _responseText + "\n");
            sb.Append("Transaction Date : " + _transactionDate + "\n");
            sb.Append("Transaction Time : " + _transactionTime + "\n");
            sb.Append("Terminal ID : " + _terminalID + "\n");
            sb.Append("Card Number : " + _cardNumber + "\n");
            sb.Append("Expiry Date : " + _expiryDate + "\n");
            sb.Append("Amount Transaction : " + _amountTransaction + "\n");
            sb.Append("Amount TIP : " + _amountTip + "\n");
            sb.Append("Batch Number : " + _batchNumber + "\n");
            sb.Append("Batch Amount : " + _batchAmount + "\n");
            sb.Append("Invoice Number : " + _invoiceNumber + "\n");
            sb.Append("Merchant Name And Address : " + _merchantNameAndAddress + "\n");
            sb.Append("Merchant Number : " + _merchantNumber + "\n");
            sb.Append("Card Issuer Name : " + _cardIssuerName + "\n");
            sb.Append("Retrieval Reference Number : " + _retrievalReferenceNumber + "\n");
            sb.Append("Card Issuer ID : " + _cardIssuerID + "\n");
            sb.Append("Card Holder Name : " + _cardHolderName + "\n");
            sb.Append("System Trace No : " + _systemTraceNo + "\n");
            sb.Append("Batch Total : " + _batchTotal + "\n");
            sb.Append("NII : " + _nii + "\n");

            return sb.ToString();
        }
    }
}
