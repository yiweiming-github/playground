using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YieldChain.CTP;

namespace CTPMarketProvider
{
    class CTPMarketAdapter
    {
        public CTPMarketAdapter(string address, string brokerId, string userId, string password)
        {
            _address = address;
            _brokerId = brokerId;
            _userId = userId;
            _password = password;
        }

        public IMarketDataProcessor DataProcesser
        {
            private get
            {
                return _dataProcessor;
            }

            set
            {
                _dataProcessor = value;
            }
        }

        public void Start()
        {
            _dataApi = new MdApi(CtpInterType.standard, "", false, false);
            _dataApi.OnFrontEvent += DataApi_OnFrontEvent;
            _dataApi.OnRspEvent += DataApi_OnRspEvent;
            _dataApi.OnRtnEvent += DataApi_OnRtnEvent;

            _dataApi.RegisterFront(_address);
            _dataApi.Init();
        }

        void DataApi_OnRtnEvent(object sender, OnRtnEventArgs e)
        {
            //Console.WriteLine("DataApi_OnRtnEvent " + e.EventType.ToString());

            var fld = e.ToField<ThostFtdcDepthMarketDataField>();

            Console.WriteLine("{0}.{1:D3} {2} {3}", fld.UpdateTime, fld.UpdateMillisec, fld.InstrumentID, fld.LastPrice);
            _dataProcessor?.Process(fld);
        }

        bool IsError(ThostFtdcRspInfoField rspinfo, string source)
        {
            if (rspinfo.ErrorID != 0)
            {
                Console.WriteLine(rspinfo.ErrorMsg + ", 来源 " + source);
                return true;
            }
            return false;
        }

        void DataApi_OnRspEvent(object sender, OnRspEventArgs e)
        {
            Console.WriteLine("DataApi_OnRspEvent " + e.EventType.ToString());
            bool err = IsError(e.RspInfo, e.EventType.ToString());

            switch (e.EventType)
            {
                case EnumOnRspType.OnRspUserLogin:
                    if (!err)
                    {
                        Console.WriteLine("登录成功");
                        var res = _dataApi.SubscribeMarketData(new[] { "rb1905" });
                        Console.WriteLine("SubscribeMarketData:" + res);
                        Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(10000);
                            res = _dataApi.SubscribeMarketData(new[] { "rb1905" });
                            Console.WriteLine("SubscribeMarketData:" + res);
                        });
                    }
                    break;
                case EnumOnRspType.OnRspSubMarketData:
                    {
                        var f = e.ToField<ThostFtdcSpecificInstrumentField>();
                        Console.WriteLine("订阅成功:" + f.InstrumentID);
                    }
                    break;
                case EnumOnRspType.OnRspUnSubMarketData:
                    {
                        var f = e.ToField<ThostFtdcSpecificInstrumentField>();
                        Console.WriteLine("退订成功:" + f.InstrumentID);
                    }
                    break;
            }
        }

        void DataApi_OnFrontEvent(object sender, OnFrontEventArgs e)
        {
            switch (e.EventType)
            {
                case EnumOnFrontType.OnFrontConnected:
                    {
                        var req = new ThostFtdcReqUserLoginField();
                        req.BrokerID = _brokerId;
                        req.UserID = _userId;
                        req.Password = _password;
                        int iResult = _dataApi.ReqUserLogin(req);
                    }
                    break;
            }
        }

        MdApi _dataApi;
        IMarketDataProcessor _dataProcessor;
        readonly string _address;
        readonly string _brokerId;
        readonly string _userId;
        readonly string _password;
    }
}
