using HoangThach.Data.EF.HoangThach.DeliveryOperation;
using HoangThach.Data.Entities;
using HoangThach.Data.Entities.HoangThach.Card;
using HoangThach.Data.Entities.HoangThach.DeliveryOperation;
using HoangThach.Data.Entities.HoangThach.Led;
using HoangThach.Data.Entities.HoangThach.Plc;
using HoangThach.Data.Entities.HoangThach.XMHT;
using HoangThach.Data.Enums;
using HoangThach.DeliveryOperation.Data;
using HoangThach.DeliveryOperation.Data.Models;
using HoangThach.MessageQueue.Printers;
using HoangThach.Services.Printers;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
//using DateTime = System.DateTime;

namespace HoangThach.DeliveryOperation.Services
{

    public partial class DeliveryUtils
    {
        private static DeliveryUtils instance = new DeliveryUtils();
        private DeliveryUtils()
        {

        }
        public static DeliveryUtils getInstance()
        {
            return instance;
        }
        #region Các Services
        public XhsContext XhsContext
        {
            get
            {
                //if (_xhsContext == null) _xhsContext = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<XhsContext>();
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<XhsContext>();
            }
        }
        XhsServices _xhsServices;
        public XhsServices XhsServices
        {
            get
            {
                if (_xhsServices == null) _xhsServices = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<XhsServices>();
                return _xhsServices;
            }
            set { _xhsServices = value; }
        }
        public LedServices LedServices
        {
            get
            {
                //if(_ledServices==null)
                //     _ledServices= ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<LedServices>();
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<LedServices>();
            }
        }
        public IPlcServices PlcServices
        {
            get
            {
                //if (_plcServices == null)
                //    _plcServices = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IPlcServices>();
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IPlcServices>();
            }
        }
        public PrinterVJ1530Services PrinterVJ1530Service
        {
            get
            {
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<PrinterVJ1530Services>();
            }
        }

        public ReaderServices ReaderServices
        {
            get
            {
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ReaderServices>();
            }
        }

        public IHubContext<DeliveryHub> hubContext
        {
            get
            {
                return ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IHubContext<DeliveryHub>>();
            }
        }
        #endregion

        #region Các biến toàn cục
        /// <summary>
        /// DS các máy in nhận được từ Message Queue
        /// </summary>
        public List<MQPrinterOut> MQPrinters = new List<MQPrinterOut>();
        public List<WeightDevice> WeighingMachines = new List<WeightDevice>();
        /// <summary>
        /// DS thẻ trong máng xuất: readerId, ds thẻ
        /// </summary>
        public List<CardNumInDeliveryGate> ListCardNumInReader;
        public List<GateOperationViewModel> GateOperationsList;
        public IServiceProvider ServiceProvider;
        //public Z3CardReaderId Z3CardReaderIds;
        //public List<> ListCardReaderIds;
        //public List<long> DeliveryGate_ReaderIds;
        public List<int> DeliveryGate_LocationIds;
        public List<DeliveryGatePlcModel> ListDeliveryGatePlc;
        //Dùng rollback soPhieuOn khi có lỗi thì lưu lại để xử ly đến khi được:DbMangXuatEnum.HMI_SoPhieuOn=5
        //public List<(int deliveryGateId, int gatePlcDatablockId, int phieuOnDbTagNum, uint value, int DataBlockType)> PlcOnRollback = new List<(int deliveryGateId, int gatePlcDatablockId, int phieuOnDbTagNum, uint value, int DataBlockType)>();

        public List<WaitingPlcCommandModel> WaitingPlcCommands = new List<WaitingPlcCommandModel>();
        /// <summary>
        /// DS máng xuất: lưu sự kiện thiết bị từ MQ, sự kiện thẻ nhận được khi có tác động cảm biến
        /// </summary>
        public List<DeliveryGateCardDetect> DeliveryGateCardDetects = new List<DeliveryGateCardDetect>();

        /// <summary>
        /// Ds bảng điện tử theo máng, chuỗi in lần cuối
        /// </summary>
        public List<DeliveryGateLed> DeliveryGateLeds;
        /// <summary>
        /// Lịch sử in ra Led
        /// </summary>
        public List<LedPrintHistory> LedPrintHistories = new List<LedPrintHistory>();
        /// <summary>
        /// Hàng đợi các sự kiện thay đổi tác động sensor để lưu db
        /// </summary>
        public ConcurrentQueue<(int DeliveryGate_Id, DateTime When, bool? Sensor1, bool? Sensor2)> Queues_SenSorEvents = new ConcurrentQueue<(int DeliveryGate_Id, DateTime When, bool? Sensor1, bool? Sensor2)>();

        /// <summary>
        /// Hàng đợi sự kiện đầu đếm
        /// </summary>
        public ConcurrentQueue<(int DeliveryRecordId, int IndicatorNumber, int Counter1, int Counter2, int Counter3)> Queues_DeliveryRecordTracking = new ConcurrentQueue<(int DeliveryRecordId, int IndicatorNumber, int Counter1, int Counter2, int Counter3)>();
        /// <summary>
        /// Queue ghi log start stop
        /// </summary>
        public ConcurrentQueue<IoTLog> DBMangXuatQueueForLog = new ConcurrentQueue<IoTLog>();

        //Không dùng trực tiếp
        private ILogger<DeliveryUtils> _logger;
        public ILogger<DeliveryUtils> Logger
        {
            get
            {
                if (_logger == null)
                    _logger = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ILogger<DeliveryUtils>>();
                return _logger;
            }

        }

        /// <summary>
        /// DS máng xuất Id loại có 1 ăng ten, không cảm biến vị trí: máng 9 (Id=16)
        /// </summary>
        public List<int> OneAntena_NoPosition_Gates = new List<int>();
        /// <summary>
        /// Thời gian chờ kết thúc tự động nếu đủ lượng và dừng xuất
        /// </summary>
        public int WaitTimeInMinuteToFinishRecord = 15;
        /// <summary>
        /// Gid dùng cho máy, định danh người dùng là máy tự chạy ngầm
        /// </summary>
        public Guid? MachineGid = null;

        public List<TestGate> TestGateList = new();

        #endregion


        public void LoadDeliveryGateLeds()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                try
                {
                    this.DeliveryGateLeds = xhs.DeliveryGateLeds
                    ?.Where(w => w.Status == (byte)HTStatusCodeEnum.Stable)?.ToList();
                    LedPrintHistories = new List<LedPrintHistory>();
                    if (DeliveryGateLeds == null || DeliveryGateLeds?.Count <= 0) return;

                    //2021-07-29 thêm LedType để gửi theo loại LED
                    var leds = xhs.Leds?.Where(w => w.Status == (byte)HTStatusCodeEnum.Stable)?.ToList();
                    if (leds == null || leds?.Count <= 0) return;
                    this.DeliveryGateLeds.ForEach(fe => fe.LedType = leds.FirstOrDefault(f => f.Id == fe.LedId)?.LedType);
                }
                catch { }
            }
        }
        /// <summary>
        /// Nạp ds máng xuất đầu đọc để chứa sk thẻ
        /// </summary>
        public void LoadCardReaderListForGate()
        {
            this.DeliveryGateCardDetects = new List<DeliveryGateCardDetect>();
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                var iotDataServices = scope.ServiceProvider.GetRequiredService<IoTDataServices>();
                try
                {
                    var deliveryGates = xhs.DeliveryGate
                                        .Where(w => w.Status == (byte)HTStatusCodeEnum.Stable)
                                         ?.ToList();
                    if (deliveryGates == null) return;

                    var plcGates = (from dp in xhs.DeliveryGate_Plc
                                    join dbk in xhs.DataBlocks on dp.PlcDataBlocId equals dbk.Id
                                    where dp.Status == (int)HTStatusCodeEnum.Stable
                                    && dbk.Status == (int)HTStatusCodeEnum.Stable
                                    select new { dp.DeliveryGateId, dp.PlcDataBlocId, dbk.DbType })?.ToList();
                    //DS ăng ten
                    var readerLocations = iotDataServices.GetAllReaders();

                    foreach (var gate in deliveryGates)
                    {
                        var gate_Locations = xhs.DeliveryGateCardReaders
                            .Where(w => w.DeliveryGateId == gate.Id && w.Status == (int)HTStatusCodeEnum.Stable && w.LocationId > 0)
                            ?.Select(s => new { s.DeliveryGateId, s.LocationId })
                            ?.Distinct()?.ToList();

                        var readerOfGate = readerLocations?.Where(w => gate_Locations?.Any(a => a.LocationId == w.Location_Id) ?? false)
                            ?.Select(s => s.Reader_Id)?.Distinct()?.ToList();

                        var plc = plcGates?.FirstOrDefault(f => f.DeliveryGateId == gate.Id);

                        var deliveryGateCardDetect = new DeliveryGateCardDetect
                        {
                            DeliveryGate_Id = gate.Id,
                            RegionId = xhs.DeliveryGate_Regions?.FirstOrDefault(f => f.DeliveryGateId == gate.Id)?.RegionId,
                            ReaderIds = readerOfGate ?? (new List<int>()),
                            LocationIds = gate_Locations?.Select(s => s.LocationId ?? 0)?.Distinct()?.ToList() ?? (new List<int>()),
                            NumberOfReaders = readerOfGate?.Count() ?? 0,
                            PackingMachineId = xhs.PackingMachineGate?.FirstOrDefault(f => f.DeliveryGateId == gate.Id)?.PackingMachineId,
                            PlcDbTypeId = plc?.DbType ?? 0,
                            PlcDataBlockId = plc?.PlcDataBlocId ?? 0,
                            TestOnGateId = gate.TestOnGateId
                        };

                        this.DeliveryGateCardDetects.Add(deliveryGateCardDetect);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Gửi lệnh in số đếm/cân tới bảng đt
        /// </summary>
        public void PrintQtyToLed()
        {
            try
            {
                //Hiển thị số bao/cân lên led
                if (GateOperationsList != null
                    && GateOperationsList.Count > 0
                    && DeliveryGateLeds != null
                    && DeliveryGateLeds.Count > 0)
                {
                    //lấy object Api Led
                    var ledServices = LedServices;
                    if (ledServices == null) return;

                    foreach (var item in GateOperationsList)
                    {
                        //DS led cho máng
                        var ledForGate = DeliveryGateLeds
                            .Where(w => w.DeliveryGateId == item.DeliveryGate_Id)?.ToList();

                        if (ledForGate == null || (ledForGate?.Count <= 0)) continue;

                        bool xiBaoPrintEnable = (item.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && item.DeliveryRecord_Status == (int)HTStatusCodeEnum.Stable);
                        bool canBanXMRPrintEnable = item.Gate_PackingType_Id == (int)HTPackingTypeEnum.ROI;// && item.DBCanXaHang != null;
                        bool xMRThuyPrintEnable = item.Gate_PackingType_Id == (int)HTPackingTypeEnum.ROI && item.Transport_Method_Id == (int)TransportMethodEnum.THUY;// && item.DBCanBangXMR != null;

                        if (!(xiBaoPrintEnable || canBanXMRPrintEnable || xMRThuyPrintEnable)) continue;

                        var deviceOfGate = DeliveryGateCardDetects?.FirstOrDefault(f=>f.DeliveryGate_Id==item.DeliveryGate_Id);
                        if (deviceOfGate?.PlcTagsMapping == null) continue;

                        foreach (var led in ledForGate)
                        {
                            //Nếu khác lần in trước thì gửi in ra led
                            var ledPrintHistory = LedPrintHistories.FirstOrDefault(f => f.LedId == led.LedId);

                            string printLedStr = "";
                            string luuLuongXMR_Thuy = "";

                            if (item.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO)
                            {
                                //printLedStr = $"{item.CurrentCountQty}/{item.DBMangXuat?.HMI_Luong_Dat ?? item.DBMangXuat_V3?.LuongBaoSauThemBot}";//SetPointQty_Bags
                                printLedStr = $"{item.CurrentCountQty}/{deviceOfGate.PlcTagsMapping.LuongDatSauThemBot}";//SetPointQty_Bags
                            }
                            else if (canBanXMRPrintEnable)
                            {
                                /*
                                if (item.DeliveryRecord_Status == (byte)HTStatusCodeEnum.Stable)
                                    printLedStr = $"{(item.DBCanXaHang.Chithican > item.Weight_1 ? item.DBCanXaHang.Chithican - item.Weight_1 : 0)}/{item.SetPointQty_Kg}";// - item.DBCanXaHang.HMI_Luong_Bi
                                else printLedStr = item.DBCanXaHang.Chithican.ToString();*/
                                if (item.DeliveryRecord_Status == (byte)HTStatusCodeEnum.Stable)
                                    printLedStr = $"{(deviceOfGate.PlcTagsMapping.ChiThiCan > item.Weight_1 ? deviceOfGate.PlcTagsMapping.ChiThiCan - item.Weight_1 : 0)}/{item.SetPointQty_Kg}";// - item.DBCanXaHang.HMI_Luong_Bi
                                else printLedStr = deviceOfGate.PlcTagsMapping.ChiThiCan.ToString();
                            }
                            else if (xMRThuyPrintEnable)
                            {
                                /*
                                if (item.DeliveryRecord_Status == (int)HTStatusCodeEnum.Stable)
                                {
                                    printLedStr = $"{((item.DBCanBangXMR.Tong_tichluy > item.Weight_1 ? item.DBCanBangXMR.Tong_tichluy - item.Weight_1 : 0) / 1000.0).ToString("N2", HoangThach.Data.Utils.AppSharedUtils.getInstance().VnCultureInfo)} t";
                                }
                                else printLedStr = "00";
                                luuLuongXMR_Thuy = $"{(item.DBCanBangXMR.Feedrate > 0 ? (item.DBCanBangXMR.Feedrate / 1000.0).ToString("N2", HoangThach.Data.Utils.AppSharedUtils.getInstance().VnCultureInfo) : 0)} t/h";
                                */
                                if (item.DeliveryRecord_Status == (int)HTStatusCodeEnum.Stable)
                                {
                                    printLedStr = $"{((deviceOfGate.PlcTagsMapping.SoTichLuy > item.Weight_1 ? deviceOfGate.PlcTagsMapping.SoTichLuy - item.Weight_1 : 0) / 1000.0).ToString("N2", HoangThach.Data.Utils.AppSharedUtils.getInstance().VnCultureInfo)} t";
                                }
                                else printLedStr = "00";
                                luuLuongXMR_Thuy = $"{(deviceOfGate.PlcTagsMapping.Feedrate > 0 ? (deviceOfGate.PlcTagsMapping.Feedrate / 1000.0).ToString("N2", HoangThach.Data.Utils.AppSharedUtils.getInstance().VnCultureInfo) : 0)} t/h";
                            }

                            //Nếu có thay đổi thì in
                            if ((led.LedType != (int)LedTypeEnum.Type1 && string.IsNullOrWhiteSpace(ledPrintHistory?.NoiDung3))
                                || (ledPrintHistory != null
                                    && ((led.LedType != (int)LedTypeEnum.Type1 && ledPrintHistory.NoiDung3 != printLedStr)
                                        || (led.LedType == (int)LedTypeEnum.Type1 && ledPrintHistory.NoiDung2 != printLedStr)
                                        //|| (item.DBCanBangXMR != null && ledPrintHistory?.NoiDung2 != luuLuongXMR_Thuy))
                                        || (xMRThuyPrintEnable && ledPrintHistory?.NoiDung2 != luuLuongXMR_Thuy))
                                    )
                                )
                            {

                                LedReQuestTongQuat ledReQuestTQ = new LedReQuestTongQuat
                                {
                                    LedId = led.LedId,
                                    reSetNoidung = false,

                                    //listNoiDung = new List<Content> { new Content { Vitridong = 3, Noidung = printLedStr } }
                                };

                                if (xMRThuyPrintEnable)
                                {
                                    ledReQuestTQ.listNoiDung.Add(new Content { Vitridong = 2, Noidung = luuLuongXMR_Thuy });

                                    if (led.LedType != (int)LedTypeEnum.Type1)
                                        ledReQuestTQ.listNoiDung.Add(new Content { Vitridong = 3, Noidung = printLedStr });
                                }
                                else if (led.LedType == (int)LedTypeEnum.Type1)
                                    ledReQuestTQ.listNoiDung = new List<Content> { new Content { Vitridong = 2, Noidung = printLedStr } };
                                else
                                    ledReQuestTQ.listNoiDung = new List<Content> { new Content { Vitridong = 3, Noidung = printLedStr } };

                                Task.Run(async () => await ledServices.PrintToLed_Genaral(ledReQuestTQ, null));
                            }
                            //Cập nhật
                            if (ledPrintHistory == null) LedPrintHistories.Add(new LedPrintHistory { LedId = led.LedId, NoiDung2 = luuLuongXMR_Thuy, NoiDung3 = printLedStr, When = DateTime.Now });
                            else
                            {
                                //if (item.DeliveryRecord_Status == (byte)HTStatusCodeEnum.Stable && item.PackingType_Id == (int)HTPackingTypeEnum.ROI && item.Transport_Method_Id == (int)TransportMethodEnum.THUY)
                                if (xMRThuyPrintEnable)
                                {
                                    ledPrintHistory.NoiDung2 = luuLuongXMR_Thuy;
                                    ledPrintHistory.NoiDung3 = printLedStr;
                                }
                                else if (item.DeliveryRecord_Status == (byte)HTStatusCodeEnum.Stable && led.LedType == 1)
                                    ledPrintHistory.NoiDung2 = printLedStr;
                                else ledPrintHistory.NoiDung3 = printLedStr;

                                ledPrintHistory.When = DateTime.Now;
                            }

                        }
                    }

                }
            }
            catch (Exception ex) { Debug.WriteLine($"PrintQtyToLed: {ex.Message}"); }
        }


        public string DecodeAlarmToString(int alarmCode)
        {
            if (alarmCode <= 0) return "";
            string msgAlarm = "";
            List<int> alarmLists = new List<int>();

            if (alarmCode < 1000)
                for (int i = 0; i < 8; i++)
                {
                    int code = alarmCode;
                    bool bitx = ((code >> i) & 1) != 0;
                    //bool bit = (i > 0 ? code & (1 << i - 1) : code & 1) != 0;
                    if (bitx)
                    {
                        alarmLists.Add((int)Math.Pow(2, i));
                        /*
                        switch (i)
                        {
                            case 0:// 1
                                   msgAlarm += $"Dính bao DD1, ";                                
                                break;
                            case 1:// 2
                                msgAlarm += $"Dính bao DD2, ";
                                break;
                            case 2://4
                                msgAlarm += $"Dính bao DD3, ";
                                break;
                            case 3://8
                                msgAlarm += $"Mất nguồn, ";
                                break;
                            case 4://16
                                msgAlarm += $"Lệch bao DD1, ";
                                break;
                            case 5://32
                                msgAlarm += $"Lệch bao DD2, ";
                                break;
                            case 6://64
                                msgAlarm += $"Lệch bao DD3, ";
                                break;
                            case 7://128
                                msgAlarm += $"Quá số lỗi quy định, ";
                                break;
                            default: break;
                        }
                        */
                    }
                }
            else alarmLists.Add(alarmCode);

            if (alarmLists.Count > 0)
            {
                var alarmDic = XhsContext.Alarms?.ToList();
                if (alarmDic?.Count > 0)
                    alarmLists.ForEach(fe =>
                    {
                        var al = alarmDic.FirstOrDefault(f => f.Id == fe);
                        msgAlarm += $"{(al == null ? fe : al.Description)};";

                    });

                //var alarms = XhsContext.Alarms?.Where(w => alarmLists.Any(a => a == w.Id))?.Select(s => s.Description).ToArray();
                //msgAlarm = String.Join(';', alarms);
            }

            return msgAlarm;
        }

        #region SaveCardEvent
        public bool savingCardEvt = false;
        /// <summary>
        /// Lưu sk thẻ từ MQ đến để xử lý dần, trên 1 luồng ktra
        /// </summary>
        //public ConcurrentQueue<CardEvent> CardEventsQueue = new ConcurrentQueue<CardEvent>();
        public ConcurrentQueue<CardEventMQ> CardEventsQueue = new ConcurrentQueue<CardEventMQ>();
        /// <summary>
        /// Lưu sự kiện thẻ vào ds nếu cảm biến máng tương ứng có tác động
        /// 20221223 dùng locationId thay readerid
        /// </summary>
        /// <param name="cardEvent"></param>
        public void SaveCardEventInGate()
        {
            CardEventMQ cardEvent;
            //            bool debug = false;
            //#if DEBUG
            //            debug = true;
            //#endif
            try
            {
                int imax = 10, i = 0;
                while (CardEventsQueue.TryDequeue(out cardEvent))
                {
                    //DS máng-đầu đọc-thẻ-xe có cùng đầu đọc với sự kiện thẻ
                    var gates = this.DeliveryGateCardDetects?
                        .Where(w => w.LocationIds?.Any(f => f == cardEvent.LocationId) ?? false)
                        ?.ToList();
                    if (gates == null)
                    {
                        cardEvent = null;
                        continue;
                    }

                    //Ktra thẻ có ở máng nào khác máng hiện tại đã được áp biển xe không, nếu có rồi thì bỏ qua(được áp khi tạo phiếu)
                    //DS các máng khác đầu đọc
                    var otherGates = this.DeliveryGateCardDetects?.Where(w => !gates.Any(a => a.DeliveryGate_Id == w.DeliveryGate_Id))
                        ?.ToList();
                    bool checkCardInOther = false;
                    if (otherGates != null)
                        foreach (var g in otherGates)
                        {
                            //Thẻ được tìm thấy ở máng khác được đánh dấu gán thủ công (đang xuất)
                            if (g.DetectedVehicleCards?.FirstOrDefault(a => a.ManualSelected && a.CardNumber == cardEvent.CardNumber) != null)
                            {
                                checkCardInOther = true;
                                break;
                            }
                        }
                    //Nếu được đánh dấu thủ công khi tạo phiếu rồi thì bỏ qua thẻ này
                    if (checkCardInOther)
                    {
                        cardEvent = null;
                        continue;
                    }

                    int hourExpiredOfCardEvents = 1, keepingmilliSecond = 1500;
                    //Duyet qua các máng có đầu đọc
                    foreach (var gate in gates)
                    {
                        //sensor tác động sau 1.5s thì lấy
                        bool sensorActive = (gate.LastDetected > gate.LastNotDetected && gate.LastNotDetected.AddMilliseconds(keepingmilliSecond) <= DateTime.Now);
                        //mất tác động trong khoảng 30s vẫn lấy
                        bool sensorDeActive = (gate.LastNotDetected > gate.LastDetected && gate.LastDetected.AddSeconds(30) > DateTime.Now);

                        bool isOneAntenGate = DeliveryUtils.getInstance().OneAntena_NoPosition_Gates.Any(a => a == gate.DeliveryGate_Id);
                        //Nếu có xe, cảm biến vị trí bị tác động thì lưu thẻ
                        //Với máng có 1 đầu đọc thì ktra tác động sensor
                        if (
                            //Có 1 ăng ten thì xem xét tác động cảm biến, trừ máng 9
                            //Có tác động đủ lâu keepingSecond (=1.5s), người đi qua không tính
                            (gate.NumberOfReaders <= 1
                                && !isOneAntenGate
                                //Thấy tác động đủ lâu
                                && (sensorActive || sensorDeActive))
                            //Máng 9 có 1 ăng ten, không có cảm biến vị trí
                            || (gate.NumberOfReaders <= 1 && isOneAntenGate)

                            //có 2 ăng ten thì không phụ thuộc cảm biến vị trí
                            || gate.NumberOfReaders > 1

                            )
                        {
                            if (gate.DetectedVehicleCards == null) gate.DetectedVehicleCards = new List<DetectedVehicleCard>();
                            //Thẻ đã phát hiện trước đó chưa
                            var detectvehiclecard = gate.DetectedVehicleCards?.FirstOrDefault(f => f.CardNumber == cardEvent.CardNumber);
                            if (detectvehiclecard == null)
                            {
                                lock (gate.DetectedVehicleCards)
                                    gate.DetectedVehicleCards.Add(new DetectedVehicleCard
                                    {
                                        CardNumber = cardEvent.CardNumber,
                                        When = cardEvent.LastDetectDate.Value,
                                        FirtTimeDetected = DateTime.Now,
                                        Sum_DetectedNumber = 1,
                                        Sum_DetectedSeconds = 1,
                                        ReaderId = cardEvent.ReaderId,
                                        ReaderIndex = cardEvent.ReaderIndex,
                                        LocationId = cardEvent.LocationId
                                    });
                            }
                            else
                            {
                                lock (detectvehiclecard)
                                {
                                    double seconds = (cardEvent.LastDetectDate.Value - detectvehiclecard.When).TotalSeconds;
                                    if (seconds > 0)
                                    {
                                        detectvehiclecard.Sum_DetectedNumber += 1;
                                        detectvehiclecard.Sum_DetectedSeconds += seconds;
                                    }
                                    detectvehiclecard.ReaderId = cardEvent.ReaderId;
                                    detectvehiclecard.ReaderIndex = cardEvent.ReaderIndex;
                                    detectvehiclecard.When = cardEvent.LastDetectDate.Value;
                                    detectvehiclecard.LocationId = cardEvent.LocationId;
                                }
                            }

                        }
                    }
                    cardEvent = null;
                    i++;
                    if (i > imax) break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi SaveCardEventInGate: {ex.Message}");
            }
        }
        /*
        /// <summary>
        /// 20221223 backup dùng readerId
        /// </summary>
        public void SaveCardEventInGate_BK()
        {
            CardEventMQ cardEvent;
            try
            {
                int imax = 10, i = 0;
                while (CardEventsQueue.TryDequeue(out cardEvent))
                {
                    //DS máng-đầu đọc-thẻ-xe có cùng đầu đọc với sự kiện thẻ
                    var gates = DeliveryGateCardDetects?
                        .Where(w => w.ReaderIds?.Count(f => f == cardEvent.ReaderId) > 0)
                        ?.ToList();
                    if (gates == null)
                    {
                        cardEvent = null;
                        continue;
                    }

                    //Ktra thẻ có ở máng nào khác máng hiện tại đã được áp biển xe không, nếu có rồi thì bỏ qua(được áp khi tạo phiếu)
                    //DS các máng khác đầu đọc
                    var otherGates = DeliveryGateCardDetects?.Where(w => !gates.Any(a => a.DeliveryGate_Id == w.DeliveryGate_Id))
                        ?.ToList();
                    bool checkCardInOther = false;
                    if (otherGates != null)
                        foreach (var g in otherGates)
                        {
                            //Thẻ được tìm thấy ở máng khác được đánh dấu gán thủ công (đang xuất)
                            if (g.DetectedVehicleCards?.FirstOrDefault(a => a.ManualSelected && a.CardNumber == cardEvent.CardNumber) != null)
                            {
                                checkCardInOther = true;
                                break;
                            }
                        }
                    //Nếu được đánh dấu thủ công khi tạo phiếu rồi thì bỏ qua thẻ này
                    if (checkCardInOther)
                    {
                        cardEvent = null;
                        continue;
                    }

                    int hourExpiredOfCardEvents = 1, keepingmilliSecond = 1500;
                    //Duyet qua các máng có đầu đọc
                    foreach (var gate in gates)
                    {
                        //sensor tác động sau 1.5s thì lấy
                        bool sensorActive = (gate.LastDetected > gate.LastNotDetected && gate.LastNotDetected.AddMilliseconds(keepingmilliSecond) <= DateTime.Now);
                        //mất tác động trong khoảng 30s vẫn lấy
                        bool sensorDeActive = (gate.LastNotDetected > gate.LastDetected && gate.LastDetected.AddSeconds(30) > DateTime.Now);

                        bool isOneAntenGate = DeliveryUtils.getInstance().OneAntena_NoPosition_Gates.Any(a => a == gate.DeliveryGate_Id);
                        //Nếu có xe, cảm biến vị trí bị tác động thì lưu thẻ
                        //Với máng có 1 đầu đọc thì ktra tác động sensor
                        if (
                            //Có 1 ăng ten thì xem xé tác động cảm biến, trừ máng 9
                            //Có tác động đủ lâu keepingSecond (=1.5s), người đi qua không tính
                            (gate.ReaderIds?.Count <= 1
                                && !isOneAntenGate
                                //Thấy tác động đủ lâu
                                && (sensorActive || sensorDeActive))
                            //Máng 9 có 1 ăng ten, không có cảm biến vị trí
                            || (gate.ReaderIds?.Count <= 1 && isOneAntenGate)

                            //có 2 ăng ten thì không phụ thuộc cảm biến vị trí
                            || gate.ReaderIds?.Count > 1

                            //&& deliveryGate != null
                            //Những máng đang có phiếu đang xuất thì bỏ qua
                            //&& deliveryGate.DeliveryRecord_Status != (byte)HTStatusCodeEnum.Stable
                            )
                        {
                            if (gate.DetectedVehicleCards == null) gate.DetectedVehicleCards = new List<DetectedVehicleCard>();
                            //Thẻ đã phát hiện trước đó chưa
                            var detectvehiclecard = gate.DetectedVehicleCards?.FirstOrDefault(f => f.CardNumber == cardEvent.CardNumber);
                            if (detectvehiclecard == null)
                            {
                                lock (gate.DetectedVehicleCards)
                                    gate.DetectedVehicleCards.Add(new DetectedVehicleCard
                                    {
                                        CardNumber = cardEvent.CardNumber,
                                        When = cardEvent.LastDetectDate.Value,
                                        FirtTimeDetected = DateTime.Now,
                                        Sum_DetectedNumber = 1,
                                        Sum_DetectedSeconds = 1,
                                        ReaderId = cardEvent.ReaderId,
                                        ReaderIndex = cardEvent.ReaderIndex,
                                        LocationId = cardEvent.LocationId
                                    });
                            }
                            else
                            {
                                lock (detectvehiclecard)
                                {
                                    double seconds = (cardEvent.LastDetectDate.Value - detectvehiclecard.When).TotalSeconds;
                                    if (seconds > 0)
                                    {
                                        detectvehiclecard.Sum_DetectedNumber += 1;
                                        detectvehiclecard.Sum_DetectedSeconds += seconds;
                                    }
                                    detectvehiclecard.ReaderId = cardEvent.ReaderId;
                                    detectvehiclecard.ReaderIndex = cardEvent.ReaderIndex;
                                    detectvehiclecard.When = cardEvent.LastDetectDate.Value;
                                    detectvehiclecard.LocationId = cardEvent.LocationId;
                                }
                            }

                        }
                    }
                    cardEvent = null;
                    i++;
                    if (i > imax) break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi SaveCardEventInGate: {ex.Message}");
            }
        }
        */
        #endregion
        public void ShowVehicleDetectedToLedTV(int deliveryGateId)
        {
            //HIển thị led số xe đọc được khi máng không xuất,sau khi kết thúc 5s
            var LedSvr = DeliveryUtils.getInstance().LedServices;

            //if (LedSvr == null) return;
            //DS led cho máng
            var ledForGate = DeliveryUtils.getInstance().DeliveryGateLeds
                ?.Where(w => w.DeliveryGateId == deliveryGateId)?.ToList();
            if (ledForGate?.Count <= 0) return;

            if ((DeliveryUtils.getInstance().GateOperationsList?
                .Any(a => a.DeliveryGate_Id == deliveryGateId
                    && a.DeliveryRecord_Status != (int)HTStatusCodeEnum.Stable
                    && a.DeliveryRecord_ModifiedDate.Value.AddSeconds(5) < DateTime.Now)
                    ) ?? false)

            {
                try
                {
                    var gate = DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
                    if (gate == null) return;

                    if (!gate.CreateNewDeliveryPermission || !gate.FinishDeliveryPermission) return;

                    //lấy 3 xe sau cùng
                    var vehicles = gate.DetectedVehicleCards
                       ?.Where(w => !string.IsNullOrEmpty(w.VehicleCode) && w.VehicleCode?.Length > 5)
                       ?.OrderByDescending(o => o.When)
                       ?.ToList();

                    if (vehicles != null && vehicles?.Count > 0)
                    {
                        var vehicle_ToLed = vehicles?.Count > 3 ? vehicles?.Take(3).ToList() : vehicles;

                        LedReQuestTongQuat ledReQuestTQ = new LedReQuestTongQuat
                        {
                            reSetNoidung = true,
                            listNoiDung = new List<Content>()
                        };
                        for (int i = 0; i < 3; i++)
                        {
                            var hasWaiting = gate.WaitingDelivery?.Any(a => a.VehicleCode == vehicle_ToLed[i].VehicleCode) ?? false;
                            //Thông báo bảng led về cân bì
                            string checkTarWeightMsg = DeliveryUtils.getInstance().TarWeighRequire && hasWaiting && !vehicle_ToLed[i].HasTarWeight ? "-Chua can bi" : "";
                            ledReQuestTQ.listNoiDung.Add(new Content
                            {
                                Vitridong = i + 1,
                                Noidung = i < vehicle_ToLed.Count ? $"{vehicle_ToLed[i].VehicleCode}{checkTarWeightMsg}" : "...",
                                CanLe = 2
                            });
                        }

                        foreach (var led in ledForGate)
                        {
                            ledReQuestTQ.LedId = led.LedId;
                            Task.Run(async () => await LedSvr.PrintToLed_Genaral(ledReQuestTQ, null));
                        }
                    }
                    else
                    {
                        LedReQuestTongQuat ledReQuestTQ = new LedReQuestTongQuat
                        {
                            reSetNoidung = true,
                            listNoiDung = new List<Content> { new Content { Vitridong=2,
                                Noidung= "Đang chờ xe",
                                CanLe=2
                            } }
                        };
                        foreach (var led in ledForGate)
                        {
                            ledReQuestTQ.LedId = led.LedId;
                            Task.Run(async () => await LedSvr.PrintToLed_Genaral(ledReQuestTQ, null));
                        }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, $"Lỗi hiển thị biển xe ra Led, máng: {deliveryGateId}"); }
            }
        }
        /// <summary>
        /// Gán biển số xe vào máng khi đăng ký nhập mới phiếu xuất
        /// Căn cứ biển xe của đơn hàng, tìm trong ds xe của máng nếu có thì đánh dấu thủ công
        /// đồng thời xóa biển xe trùng ở máng khác, xóa các xe khác của máng hiện tại
        /// </summary>
        public void ManualApplyVehiceCodeForGate(int deliveryGateId, string vehicleCode)
        {
            var gate = DeliveryGateCardDetects.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gate == null) return;
            if (gate.DetectedVehicleCards == null) gate.DetectedVehicleCards = new List<DetectedVehicleCard>();
            vehicleCode = this.VehicleCode(vehicleCode);
            //Xóa các biển xe khác biển cần áp thủ công của máng hiện tại
            if (gate.DetectedVehicleCards != null && gate.DetectedVehicleCards.Count > 0)
            {
                lock (gate.DetectedVehicleCards)
                    gate.DetectedVehicleCards?.RemoveAll(r => r.VehicleCode != vehicleCode);
            }

            //Tìm xem có trong ds chưa
            var vehicleOfGate = gate?.DetectedVehicleCards?.FirstOrDefault(f => f.VehicleCode == vehicleCode);
            //Nếu chưa có thì thêm
            lock (gate.DetectedVehicleCards)
            {
                if (vehicleOfGate == null)
                {
                    gate.DetectedVehicleCards.Add(new DetectedVehicleCard
                    {
                        CardNumber = 0,//gán số thẻ sau
                        VehicleCode = vehicleCode,
                        When = DateTime.Now,
                        ManualSelected = true,
                    });
                }
                else//Nếu có thì đánh dấu
                {
                    vehicleOfGate.ManualSelected = true;
                }
            }
            //Hủy các biển trùng ở máng khác
            var otherGate = DeliveryGateCardDetects?.Where(w => w.DeliveryGate_Id != deliveryGateId)?.ToList();
            //?.ForEach(fe => fe.DetectedVehicleCards?.RemoveAll(r => r.VehicleCode == vehicleCode));
            if (otherGate != null && otherGate.Count > 0)
                foreach (var og in otherGate)
                {
                    if (og.DetectedVehicleCards != null)
                        lock (og.DetectedVehicleCards)
                            og.DetectedVehicleCards.RemoveAll(r => r.VehicleCode == vehicleCode);
                }
        }

        public void LoadPrinter()
        {
            if (this.GateOperationsList != null
                && this.GateOperationsList.Count > 0)
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                    try
                    {
                        var prtList = (from p in xhs.Printers
                                       join pg in xhs.DeliveryGate_Printer
                                           on p.Id equals pg.PrinterId
                                       where //pg.DeliveryGateId == item.DeliveryGate_Id
                                           p.Status == (int)HTStatusCodeEnum.Stable
                                           && pg.Status == (int)HTStatusCodeEnum.Stable
                                       select new { p, pg })?.ToList();
                        if (prtList == null) return;

                        foreach (var item in this.GateOperationsList)
                        {

                            //Máy in theo máng
                            var prt1 = prtList.Where(w => w.pg.DeliveryGateId == item.DeliveryGate_Id)?.ToList();
                            if (prt1 != null && prt1.Count > 0)
                            {
                                lock (item)
                                {
                                    item.Printers?.Clear();
                                    //Reset máy in của máng
                                    item.Printers = new List<MQPrinterOut>();
                                    //Cập nhật máy in cho máng hiện tại
                                    foreach (var pr in prt1)
                                    {
                                        //Lấy thông tin máy in từ MQ
                                        var MQpr = this.MQPrinters?.FirstOrDefault(f => f.PrinterId == pr.pg.PrinterId);
                                        MQPrinterOut mQPrinter = new MQPrinterOut
                                        {
                                            PrinterId = pr.p.Id,
                                            PrinterName = pr.p.Name,
                                            IsConnected = MQpr?.IsConnected ?? false,
                                            IsPrinterOn = MQpr?.IsPrinterOn ?? false,
                                            PrintMessage = string.IsNullOrWhiteSpace(item.PrintCode) ? " " : $"{item.PrintCode}{DateTime.Now.ToString("ddMMyy")}",
                                            PrintedSuccess = MQpr?.PrintedSuccess ?? false

                                        };
                                        item.Printers.Add(mQPrinter);
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        /*
        public bool CheckCardReaderForDeliveryGate(long readerId)
        {
            bool ret = false;
            using (var scope = ServiceProvider.CreateScope())
            {
                try
                {
                    //Load ds reader
                    if (this.DeliveryGate_ReaderIds == null || DeliveryUtils.getInstance().DeliveryGate_ReaderIds?.Count <= 0)
                    {
                        var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                        this.DeliveryGate_ReaderIds = new List<long>();
                        xhs.DeliveryGateCardReaders
                            .Where(w => w.Status == (int)HTStatusCodeEnum.Stable)
                            ?.ToList()?.ForEach(fe => this.DeliveryGate_ReaderIds.Add(fe.ReaderId));
                    }

                    ret = DeliveryGate_ReaderIds.Any(a => a == readerId);

                }
                catch (Exception ex) { Debug.WriteLine($"CheckZ3CardReaderForDeliveryGate: {ex.Message}"); }

            }
            return ret;
        }
        */
        public bool CheckCardReaderForDeliveryGateByLocationId(int locationId)
        {
            bool ret = false;
            using (var scope = ServiceProvider.CreateScope())
            {
                try
                {
                    //Load ds reader
                    if (this.DeliveryGate_LocationIds == null || this.DeliveryGate_LocationIds?.Count <= 0)
                    {
                        var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();

                        var gate_Locations = xhs.DeliveryGateCardReaders
                            ?.Where(w => w.Status == (int)HTStatusCodeEnum.Stable && w.LocationId > 0)
                            ?.Select(s => s.LocationId ?? 0)
                            ?.Distinct()?.ToList();

                        this.DeliveryGate_LocationIds = gate_Locations ?? new List<int>();
                    }

                    ret = DeliveryGate_LocationIds?.Any(a => a == locationId) ?? false;

                }
                catch (Exception ex) { Debug.WriteLine($"CheckCardReaderForDeliveryGateByLocationId: {ex.Message}"); }

            }
            return ret;
        }
        /// <summary>
        /// Bật đèn giao thông
        /// </summary>
        /// <param name="deliveryGateId"></param>
        /// <param name="turnOnOff">1: Xanh; 0:Đỏ</param>
        /// <returns></returns>
        public async Task<(bool, string)> TurnTrafficLight(string accessToken, int deliveryGateId, uint turnOnOff)
        {
            var gate = DeliveryUtils.getInstance().GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gate == null) return (false, $"Không tồn tại máng {deliveryGateId}");

            int plcDatablockId = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                plcDatablockId = xhs.DeliveryGate_Plc.FirstOrDefault(f => f.DeliveryGateId == gate.DeliveryGate_Id && f.Status == (byte)HTStatusCodeEnum.Stable)?.PlcDataBlocId ?? 0;
                if (plcDatablockId <= 0) return (false, $"Không tồn tại plc máng {deliveryGateId}");

                var plcServices = scope.ServiceProvider.GetRequiredService<IPlcServices>();

                var plcDbType = DeliveryUtils.getInstance().DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId)?.PlcDbTypeId ?? 0;

                if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && plcDbType == (int)DataBlockType.DbMangXuat)
                {
                    //var dataPack = new ThaoTacPlc_V1<DbMangXuatEnum>
                    var dataPack = new WritePlcTagPkg
                    {
                        Plc_DataBlockId = plcDatablockId,
                        EnumTag = (int)DbMangXuatEnum.HMI_Lamp_out,
                        GiaTri = (int)turnOnOff,
                        //HtDataType = HTDataTypeEnum._int
                    };

                    if (!await plcServices.WritePlcTag(dataPack, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Lamp_out xuống PLC");
                    }
                }
                else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && plcDbType == (int)DataBlockType.DbMangxuat_V3)
                {
                    //var dataPack = new ThaoTacPlc_V1<DbMangXuatEnum_V3>
                    var dataPack = new WritePlcTagPkg
                    {
                        Plc_DataBlockId = plcDatablockId,
                        EnumTag = (int)DbMangXuatEnum_V3.Input_HmiLampIn,
                        GiaTri = (int)turnOnOff,
                        //HtDataType = HTDataTypeEnum._int
                    };

                    if (!await plcServices.WritePlcTag(dataPack, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Lamp_out xuống PLC");
                    }
                }
                else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.ROI)
                {
                    //var dataPackXMR = new ThaoTacPlc_V1<DbCanXaHangEnum>
                    var dataPackXMR = new WritePlcTagPkg
                    {
                        Plc_DataBlockId = plcDatablockId,
                        EnumTag = (int)DbCanXaHangEnum.HMI_Lamp_out,
                        GiaTri = (int)turnOnOff,
                        //HtDataType = HTDataTypeEnum._int
                    };

                    if (!await plcServices.WritePlcTag(dataPackXMR, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Lamp_out xuống PLC");
                    }
                }

            }
            return (true, ""); ;
        }
        /// <summary>
        /// chạy/dừng máng
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="deliveryGateId"></param>
        /// <param name="start">true: start; false:stop</param>
        /// <returns></returns>
        public async Task<(bool, string)> Start_StopGate(string accessToken, int deliveryGateId, bool start)
        {
            var gate = DeliveryUtils.getInstance().GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gate == null) return (false, $"Không tồn tại máng {deliveryGateId}");
            var gateOfDevice = this.DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gateOfDevice == null) return (false, $"Không tồn tại tb của máng {deliveryGateId}");

            string msg = "";
            //int plcDatablockId = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                try
                {
                    var plcDatablockId = ListDeliveryGatePlc?.FirstOrDefault(f => f.DeliveryGateId == deliveryGateId)?.PlcDatablockId ?? 0;
                    if (plcDatablockId <= 0) return (false, $"Không tồn tại plc máng {deliveryGateId}");

                    var plcServices = scope.ServiceProvider.GetRequiredService<IPlcServices>();
                    WritePlcTagPkg writePlcTagPkg = new WritePlcTagPkg
                    {
                        Plc_DataBlockId = plcDatablockId,
                        EnumTag = 0,
                        GiaTri = (start ? 1 : 0)
                    };


                    if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && gateOfDevice.PlcDbTypeId == (int)DataBlockType.DbMangXuat)
                    {
                        writePlcTagPkg.EnumTag = (int)DbMangXuatEnum.HMI_Start_Stop;
                    }
                    else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && gateOfDevice.PlcDbTypeId == (int)DataBlockType.DbMangxuat_V3)
                    {
                        writePlcTagPkg.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiStartStop;
                    }
                    else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.ROI && gate.Transport_Method_Id == (int)TransportMethodEnum.BO)
                    {
                        writePlcTagPkg.EnumTag = (int)DbCanXaHangEnum.HMI_Start_Stop;

                        //ktra phương tiện khi start trở lại
                        if (start && gate.DeliveryRecord_Status == (int)HTStatusCodeEnum.Stable)
                        {
                            var gateDetected = DeliveryUtils.getInstance().DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
                            if (gateDetected != null)
                            {
                                string curVehicleCode = DeliveryUtils.getInstance().VehicleCode(gate.Vehicle_Code);
                                var checkActiveVehicle = gateDetected.DetectedVehicleCards?.Any(a => a.When.AddSeconds(10) > DateTime.Now
                                                && a.VehicleCode == curVehicleCode) ?? false;
                                //Trả về true kèm warning
                                if (!checkActiveVehicle)
                                    msg = $"Đã gửi START máng, Quá lâu không nhận được thẻ của xe, hãy kiểm tra lại biển số xe cẩn thận";
                            }
                        }
                    }
                    else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.ROI && gate.Transport_Method_Id == (int)TransportMethodEnum.THUY)
                    {
                        writePlcTagPkg.EnumTag = (int)DBCanBangEnum.HMI_Start_Stop;
                    }

                    if (!await plcServices.WritePlcTag(writePlcTagPkg, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Start_Stop xuống PLC");
                    }
                }
                catch { }
            }
            return (true, msg); ;
        }

        /// <summary>
        /// Lệnh xịt khí làm sạch đầu đếm
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="deliveryGateId"></param>
        /// <returns></returns>
        public async Task<(bool, string)> CleanCounterSensor(string accessToken, int deliveryGateId)
        {
            var gate = DeliveryUtils.getInstance().GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gate == null) return (false, $"Không tồn tại máng {deliveryGateId}");
            if (gate.Gate_PackingType_Id != (int)HTPackingTypeEnum.BAO) return (false, "Chỉ dùng cho máng xuất bao");

            int plcDatablockId = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                plcDatablockId = xhs.DeliveryGate_Plc.FirstOrDefault(f => f.DeliveryGateId == gate.DeliveryGate_Id && f.Status == (byte)HTStatusCodeEnum.Stable)?.PlcDataBlocId ?? 0;
                if (plcDatablockId <= 0) return (false, $"Không tồn tại plc máng {deliveryGateId}");

                var plcServices = scope.ServiceProvider.GetRequiredService<IPlcServices>();

                var plcDbType = DeliveryUtils.getInstance().DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId)?.PlcDbTypeId ?? 0;

                WritePlcTagPkg writePlcTagPkg = new WritePlcTagPkg
                {
                    Plc_DataBlockId = plcDatablockId,
                    GiaTri = 1
                };

                if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && plcDbType == (int)DataBlockType.DbMangXuat)
                {
                    writePlcTagPkg.EnumTag = (int)DbMangXuatEnum.HMI_Lenh_Xit_Khi;
                    if (!await plcServices.WritePlcTag(writePlcTagPkg, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Lamp_out xuống PLC");
                    }
                }
                else if (gate.Gate_PackingType_Id == (int)HTPackingTypeEnum.BAO && plcDbType == (int)DataBlockType.DbMangxuat_V3)
                {
                    writePlcTagPkg.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiXitKhi;
                    if (!await plcServices.WritePlcTag(writePlcTagPkg, accessToken))
                    {
                        return (false, $"Không gửi được lệnh HMI_Lamp_out xuống PLC");
                    }
                }
            }
            return (true, ""); ;
        }


        /// <summary>
        /// Lấy số lượng điều chỉnh theo loại
        /// </summary>
        /// <param name="deliveryRecordId">Số phiếu</param>
        /// <param name="adjustmentTypeId">loại điều chỉnh: Enum XhAdjustmentTypeId</param>
        /// <returns></returns>
        public int GetAdjustmentQtyByType(int deliveryRecordId, int adjustmentTypeId)
        {
            int qty = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();

                if (xhs == null) return -1;
                var x = xhs.QtyAdjustment
                    ?.Where(w => w.DeliveryRecord_Id == deliveryRecordId && w.AdjustmentType_Id == adjustmentTypeId && w.Status == (byte)HTStatusCodeEnum.Stable)
                    ?.Sum(s => s.Qty);
                qty = x ?? 0;
            }
            return qty;
        }
        /// <summary>
        /// Tính tổng sl điều chỉnh của phiếu
        /// </summary>
        /// <param name="DeliveryRecord_Id"></param>
        /// <returns></returns>
        public int GetQtyAdjustmentByDeliveryRecord(int DeliveryRecord_Id)
        {
            int SumQtyAdj = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                var adjQtyOfRecord = xhs.QtyAdjustment?.Where(w => w.DeliveryRecord_Id == DeliveryRecord_Id && w.Status == (int)HTStatusCodeEnum.Stable)?.ToList();
                if (adjQtyOfRecord == null) return 0;
                //không lấy cẩu xuống
                var adjTypeNotDownBag = xhs.AdjustmentType?.Where(w => w.Status == (int)HTStatusCodeEnum.Stable && w.Id != (int)XhAdjustmentTypeId.MinusDownBagsCounted)?.ToList();

                SumQtyAdj = (from qa in adjQtyOfRecord
                             join r in adjTypeNotDownBag on qa.AdjustmentType_Id equals r.Id
                             select new { adjQty = qa.Qty * r.Add_Factor })?.Sum(s => s.adjQty) ?? 0;

                //SumQtyAdj = (from qa in xhs.QtyAdjustment?.Where(w => w.DeliveryRecord_Id == DeliveryRecord_Id && w.Status == (int)HTStatusCodeEnum.Stable)
                //                 //không lấy cẩu xuống
                //             join at in xhs.AdjustmentType?.Where(w => w.Status == (int)HTStatusCodeEnum.Stable && w.Id != (int)XhAdjustmentTypeId.MinusDownBagsCounted)
                //                on qa.AdjustmentType_Id equals at.Id into ret
                //             from r in ret.DefaultIfEmpty()
                //             select new { SL = qa.Qty * r.Add_Factor }
                //                 )?.Sum(s => s.SL) ?? 0;
            }
            return SumQtyAdj;
        }


        /// <summary>
        /// Ca làm việc từ ngày giờ: ca1: 6h-14h; ca2: 14h-22h; ca3: 22h-6h
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int? WorkShift(DateTime? dt)
        {
            if (dt == null) return null;
            int hour = dt?.Hour ?? 0;
            int workShift = 0;
            if (hour >= 6 && hour < 14) workShift = 1;
            else if (hour >= 14 && hour < 22) workShift = 2;
            else workShift = 3;
            return workShift;
        }
        public DateTime? WorkingDay(DateTime? dt)
        {
            if (dt == null) return null;
            int hour = dt?.Hour ?? 0;
            DateTime? workingDay = dt;
            if (hour < 6) workingDay = dt.Value.AddDays(-1);

            return workingDay;
        }
        /// <summary>
        /// Tính ngày sx, 0-6h tính vào hôm trước
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime CalculateDeliveryDate(DateTime dateTime)
        {
            DateTime ret;
            if (dateTime.Hour >= 0 && dateTime.Hour < 6) ret = dateTime.Date.AddDays(-1);
            else ret = dateTime.Date;
            return ret;
        }
        public string ShortCustomerName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return name.Replace("Công ty", "Cty")
                .Replace("TNHH", "")
                .Replace("trách nhiệm hữu hạn", "")
                .Replace("một thành viên", "")
                .Replace("thương mại", "")
                .Replace("Thương Mại", "")
                .Replace("và", "")
                .Replace("Vận Tải", "")
                 .Replace("Vận tải", "")
                .Replace("Cổ phần", "")
                .Replace("dịch vụ", "")
                .Replace(",", "")
                .Replace("kinh doanh", "")
                .Replace("Đầu tư", "")
                .Replace("xây dựng", "")
                .Trim();
        }
        public string ShortItemName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return name.Replace("Xi măng Hoàng Thạch", "")
                .Replace("Xi măng", "")
                .Replace("xây trát", "")
                .Trim();
        }
        /// <summary>
        /// Chuẩn hóa số phương tiện, chỉ gồm 1 biển đầu hoặc đuôi, loại bỏ các ký tự đặc biệt, chỉ để lại chữ và số
        /// </summary>
        /// <param name="vehicleCode">Biển số xe</param>
        /// <returns></returns>
        public string VehicleCode(string? vehicleCode)
        {
            if (string.IsNullOrEmpty(vehicleCode)) return "";
            //return vehicleCode.Replace(" ", "").Replace("-", "").Replace(".", "").Trim().ToUpper();
            return Regex.Replace(vehicleCode, "[^A-Za-z0-9]", "").ToUpper();
        }
        /// <summary>
        /// Lấy tên Nhân viên giao nhận sau cùng
        /// </summary>
        /// <returns></returns>
        public string LastDeliveryUserName(int regionId = 0, int deliveryRecordId = 0)
        {
            string ret = "";
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();

                if (deliveryRecordId > 0)//Theo phiếu
                {
                    var deliveryRecord = xhs.DeliveryRecord.FirstOrDefault(f => f.Id == deliveryRecordId);
                    ret = deliveryRecord?.DeliveryUserName;
                    if (deliveryRecord?.Status != (byte)HTStatusCodeEnum.Stable) return ret;
                }
                //Lấy theo tên người vận hành cuối cùng nếu phiếu chưa kêt thúc
                if (string.IsNullOrEmpty(ret))
                {
                    var lastDeliveryRecordHasName = (from dr in xhs.DeliveryRecord
                                                     join w in xhs.WaitDeliveryQueue
                                                      on dr.WaitDeliveryQueue_Id equals w.Id
                                                     where !string.IsNullOrEmpty(dr.DeliveryUserName) && dr.CreatedDate.AddDays(1) > DateTime.Now
                                                     && dr.Status == (int)HTStatusCodeEnum.Complete
                                                     && !dr.IsTest
                                                     && dr.DeliveryQty_Kg > 0
                                                     && w.RegionId == regionId
                                                     select new { dr.Id, dr.DeliveryUserName })?.OrderByDescending(o => o.Id).FirstOrDefault();

                    ret = lastDeliveryRecordHasName?.DeliveryUserName;
                }
            }
            return ret;
        }
        public int TaiKhoanId_FromUserClaims(ClaimsPrincipal userClaims)
        {
            int taiKhoanId = 0;
            if (userClaims != null)
            {
                string tk = userClaims.Claims.FirstOrDefault(f => f.Type == "TaiKhoanId")?.Value;
                int.TryParse(tk, out taiKhoanId);
            }
            return taiKhoanId;
        }

        public Guid? Gid_FromUserClaims(ClaimsPrincipal userClaims)
        {
            Guid? gid = null;
            if (userClaims != null)
            {
                var gidStr = userClaims.Claims?.FirstOrDefault(f => f.Type == "Gid")?.Value ?? string.Empty;
                if (Guid.TryParse(gidStr, out Guid x)) gid = x;
            }
            return gid;
        }

        /// <summary>
        /// Lấy ds đầu đọc thẻ cập nhật trạng thái đầu đọc của máng; 1 máng có hơn 1 đầu đọc thì tất cả đầu đọc connected thì coi là có kết nối
        /// </summary>
        public void GateReaderStatus()
        {
            Task.Run(async () =>
            {
                //đọc API card reader
                using (var scope = ServiceProvider.CreateScope())
                {
                    try
                    {
                        //var readerSvc = ReaderServices.GetReadersStatus().Result;
                        var readerServices = scope.ServiceProvider.GetRequiredService<ReaderServices>();
                        //var readerSvc = Task.Run(async () => await readerServices.GetReadersStatus());
                        var readerSvc = await readerServices.GetReadersStatus();
                        if (readerSvc != null && DeliveryGateCardDetects != null)
                        {
                            foreach (var gate in DeliveryGateCardDetects)
                            {
                                if (gate.ReaderIds == null) continue;

                                var operationGate = GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == gate.DeliveryGate_Id);
                                if (operationGate == null || operationGate?.DeviceStatus == null) continue;
                                //đầu đọc theo máng có đủ kết nối
                                var reader = readerSvc.Where(f => gate.ReaderIds.Contains(f.Reader.Id) && f.DeviceConnection);

                                if (reader?.Count() == gate.NumberOfReaders && gate.NumberOfReaders > 0)
                                    operationGate.DeviceStatus.CardReaderConnected = true;
                                else operationGate.DeviceStatus.CardReaderConnected = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-Error: {ex.Message}";
                        Debug.WriteLine(err);
                    }
                }
            }).Wait(2000);

        }

        /// <summary>
        /// Kiểm tra số lượng max ghi log với số lượng hiện tại, nếu sl log > sl hiện tại thì không cho mở cửa xả XMR xuất tiếp
        /// hạn chế trường hợp đang xuất, chưa kết thúc nhưng lại cho xe khác vào lấy
        /// Nếu phát hien ktra=false => gửi lệnh stop máng và khi gửi lệnh start => ktra = true mới cho thực hiện
        /// </summary>
        /// <param name="deliveryGateId">máng xuát ID</param>
        /// <returns></returns>
        public bool CheckTrackingLogQty(int deliveryGateId)
        {
            var deliveryGate = GateOperationsList.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId && f.DeliveryRecord_Status == (byte)HTStatusCodeEnum.Stable);
            if (deliveryGate == null) return true;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();

                var maxQtyTracking = xhs.DeliveryRecordTracking
                    ?.Where(w => w.DeliveryRecordId == deliveryGate.DeliveryRecord_Id)
                    ?.Max(m => m.CounterNumber);

                //Nếu lượng nhỏ thì bỏ qua với XMR
                if (deliveryGate.PackingType_Id == (int)HTPackingTypeEnum.ROI && (maxQtyTracking ?? 0) < 1000) return true;

                int maxBao = 10, maxROI = 500;
                //Nếu XM bao mà log đã ghi nhiều hơn thực tế 20 bao hoặc XMR mà log nhiều hơn thực tế 5 tấn
                // thì không cho phép
                if ((deliveryGate.PackingType_Id == (int)HTPackingTypeEnum.BAO && maxQtyTracking > deliveryGate.CurrentCountQty + maxBao)
                    || (deliveryGate.PackingType_Id == (int)HTPackingTypeEnum.ROI && maxQtyTracking > deliveryGate.CurrentCountQty + maxROI))
                    return false;
            }

            return true;
        }

        bool reloading = false;
        DateTime? lastReloadGate;
        public void Reload_GateOperationsList(bool refreshPrinter = false, Guid? gid = null)
        {
            //Nếu mới load 15s trở lại và không cần refresh thì bỏ qua
            if (!refreshPrinter && lastReloadGate?.AddSeconds(15) > DateTime.Now) return;

            string debugPosition = "";
            int loop = 0;
            //Chờ nếu có tiến trình khác gọi

            while (reloading)
            {
                loop++;
                if (loop >= 10) return;
                Thread.Sleep(100);
            }

            reloading = true;

            //Lưu giá trị hiện tại
            var oldgateOperationViewModels = this.GateOperationsList;// gateOperationViewModels;
            try
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    try
                    {
                        debugPosition = "Reload_GateOperationsList->xhsServices = scope.ServiceProvider.GetRequiredService<XhsServices>();";
                        var xhsServices = scope.ServiceProvider.GetRequiredService<XhsServices>();
                        debugPosition = "Reload_GateOperationsList->xhsServices.p_ListDeliveryGate(null)";
                        var result = xhsServices.p_ListDeliveryGate(gid);
                        lastReloadGate = DateTime.Now;
                        //cập nhật ds mới từ DB
                        if (this.GateOperationsList != null)
                            lock (this.GateOperationsList)
                                this.GateOperationsList = result;
                        else this.GateOperationsList = result;

                        debugPosition = "Reload_GateOperationsList->this.LoadPrinter();";
                        if (refreshPrinter)
                        {
                            this.LoadPrinter();
                        }

                        //Nạp lại giá trị object cũ từ MQ trước khi update ds
                        if (this.GateOperationsList != null)
                        {
                            bool firstTime = oldgateOperationViewModels == null;

                            var xhsContext = scope.ServiceProvider.GetRequiredService<XhsContext>();
                            foreach (var item in this.GateOperationsList)
                            {
                                try
                                {
                                    var oldItem = oldgateOperationViewModels?.FirstOrDefault(f => f.DeliveryGate_Id == item.DeliveryGate_Id);
                                    if (oldItem != null)
                                    {
                                        debugPosition = "Reload_GateOperationsList-> lock (item)";
                                        lock (item)
                                        {
                                            item.CurrentCountQty = oldItem.CurrentCountQty;
                                            item.DBMangXuat = oldItem.DBMangXuat;
                                            item.DBMangXuat_V3 = oldItem.DBMangXuat_V3;
                                            item.DBCanXaHang = oldItem.DBCanXaHang;
                                            item.DBCanBangXMR = oldItem.DBCanBangXMR;
                                            item.DeviceStatus = oldItem.DeviceStatus;
                                            item.WhenSaveTracking = oldItem.WhenSaveTracking;
                                            //các err msg
                                            item.ErrMsg = oldItem.ErrMsg;
                                            item.Msg = oldItem.Msg;
                                            item.CardReaderErrMsg = oldItem.CardReaderErrMsg;
                                            item.CategoryErrMsg = oldItem.CategoryErrMsg;
                                            item.CounterErrMsg = oldItem.CounterErrMsg;
                                            item.PlcErrMsg = oldItem.PlcErrMsg;
                                            item.PositionSensorErrMsg = oldItem.PositionSensorErrMsg;
                                            item.PrinterErrMsg = oldItem.PrinterErrMsg;
                                            item.VehicleCodeDetecteds = oldItem.VehicleCodeDetecteds;
                                            //nếu refresh thì bỏ qua
                                            if (!refreshPrinter && oldItem.Printers?.Count > 0)
                                                item.Printers = oldItem.Printers;
                                        }
                                    }
                                    debugPosition = "Reload_GateOperationsList->  if (oldItem?.RegionId > 0)";

                                }
                                catch { }
                            }

                        }

                        oldgateOperationViewModels?.Clear();
                        oldgateOperationViewModels = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{debugPosition}:{ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"{debugPosition}:{ex.Message}", ex);
            }
            finally { reloading = false; }

        }

        /// <summary>
        /// Lấy PlcId từ máng xuất Id
        /// </summary>
        /// <param name="deliveryGateId">Máng xuất Id</param>
        /// <returns></returns>
        public int PlcIdFromGateId(int deliveryGateId)
        {
            int plcId = 0;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                plcId = xhs.DeliveryGate_Plc.FirstOrDefault(f => f.DeliveryGateId == deliveryGateId && f.Status == (byte)HTStatusCodeEnum.Stable)?.PlcDataBlocId ?? 0;
            }
            return plcId;
        }

        public string TenMangCT34(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "";
            string name = "";
            switch (code)
            {
                case "XBJ94": name = "Máng 9"; break;
                case "XBU55":
                    name = "Máng 4"; break;
                case "XBU46":
                    name = "Máng 8"; break;
                case "XBU64":
                    name = "Máng 1"; break;
                case "XBU54":
                    name = "Máng 3"; break;
                case "CT342":
                    name = "Cân bàn CT34.2"; break;
                case "CT341":
                    name = "Cân bàn CT34.1"; break;
                case "XBU65":
                    name = "Máng 2"; break;
                case "XBU45":
                    name = "Máng 6"; break;
                case "XBU24":
                    name = " Máng Hạ lưu 1"; break;
                case "XBU10":
                    name = " Máng Trung Lưu 1"; break;
                case "XBU12":
                    name = " Máng Thượng Lưu 2"; break;
                case "XBU14":
                    name = " Máng Thượng Lưu 1"; break;
                case "XBU02":
                    name = " Máng Hạ Lưu 2"; break;
                case "XBJ95":
                    name = " Máng 10"; break;
                case "J40":
                    name = "Máng XMR thủy"; break;
            }
            return name;
        }

        public void SendToPrinter(int deliveryGateId, string printCode)
        {
            var printOfGate = XhsContext.DeliveryGate_Printer.Where(w => w.DeliveryGateId == deliveryGateId)?.ToList();
            if (printOfGate?.Count > 0)
            {
                var printerVJ1530Services = DeliveryUtils.getInstance().PrinterVJ1530Service;

                foreach (var pr in printOfGate)
                {
                    HoangThach.Data.Entities.HoangThach.Printers.PrintMessageModel printMessage = new HoangThach.Data.Entities.HoangThach.Printers.PrintMessageModel
                    {
                        PrinterId = pr.PrinterId,
                        PrintMessage = string.IsNullOrEmpty(printCode) ? " " : printCode,
                        Description = $"GateId: {deliveryGateId} - {DateTime.Now}"
                    };
                    Task.Run(async () => await printerVJ1530Services.PrintMessage(printMessage, null));
                }

            }

        }
        public async Task<bool> ResetAlarm(int deliveryGateId)
        {
            bool ret = true;
            var gate = GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (gate == null) return false;

            var deviceOfgate = DeliveryUtils.getInstance().DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
            if (deviceOfgate == null) return false;

            using (var xhs = XhsContext)
            {
                int plcDataBlockId = xhs?.DeliveryGate_Plc
                   ?.FirstOrDefault(f => f.DeliveryGateId == deliveryGateId && f.Status == (byte)HTStatusCodeEnum.Stable)
                   ?.PlcDataBlocId ?? 0;

                if (plcDataBlockId <= 0) return false;

                var plcDbType = deviceOfgate?.PlcDbTypeId ?? 0;

                var dataPack = new WritePlcTagPkg
                {
                    Plc_DataBlockId = plcDataBlockId,
                    EnumTag = (int)DbMangXuatEnum.HMI_Lenh_XoaBaoDong,
                    GiaTri = 1
                };
                if (plcDbType == (int)DataBlockType.DbMangxuat_V3)
                {
                    dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiLenhXoaBaoDong;
                }

                if (!await PlcServices.WritePlcTag(dataPack, null))
                {
                    ret = false;
                }
            }
            //20230628 nếu xoá cảnh báo nhầm lẫn xe thì tăng số lần
            if (!string.IsNullOrWhiteSpace(gate.ErrMsg) && gate.ErrMsg.Contains("nhầm lẫn xe"))
            {
                deviceOfgate.ResetVehicleListTimes += 1;
            }

            //20211230 reset các error msg

            gate.ErrMsg = gate.CardReaderErrMsg = gate.CategoryErrMsg = gate.CounterErrMsg = gate.PlcErrMsg = gate.PositionSensorErrMsg = gate.PrinterErrMsg = string.Empty;

            return ret;
        }

        /// <summary>
        /// Nhập mới/kết thúc phiếu PLC 
        /// set HMI_SoPhieuOn=0/1 và HMI_Start_Stop=1 khi  HMI_SoPhieuOn=1
        /// </summary>
        ///<param name="deliveryGateId">Máng xuất Id</param>
        ///<param name="plcDataBlockId">plc datablock của máng</param>
        /// <param name="val">0/1</param>
        /// <param name="transportMethod">Phương thức vận chuyển: 1-Bộ; 2-Thủy; </param>
        /// <param name="autoCreate">0/1</param>
        /// <param name="startOn">Có start gate luôn không</param>
        /// <returns></returns>
        public async Task ChangePlc_HMI_SoPhieuOn(int deliveryGateId, int plcDataBlockId, int packingType, int deliveryRecordId, uint val,
            int transportMethod = (int)TransportMethodEnum.BO, bool autoCreate = false, bool isTestGate = false, bool startOn = true)
        {
            if (val != 0 && val != 1) return;

            string AccessToken = null;
            bool ret = false;
            string possitionDebug = $"deliveryGateId={deliveryGateId} - Set HMI_SoPhieuOn={val}";
            try
            {
                //không start khi autocreate=true
                bool sendStop = false;
                var deviceGate = this.DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
                deviceGate.ResetVehicleListTimes = 0;
                if (autoCreate)
                {
                    deviceGate.ManualStop = true;
                    //Không start khi hai máng cùng máy đóng và ở P3: để máng đang xuất xuất xong, không phải tạm dừng để bổ sung bao
                    //máng xuất khác cùng máy đóng ở P3
                    var otherGatesOfMachine = this.DeliveryGateCardDetects?
                        .Where(c => c.RegionId == (int)HTRegionId.P3 && c.PackingMachineId == deviceGate.PackingMachineId && c.DeliveryGate_Id != deviceGate.DeliveryGate_Id)?.ToList();
                    //số máng xuất khác đang xuất
                    int countGateInProcess = GateOperationsList?.Count(c => otherGatesOfMachine != null
                      && (otherGatesOfMachine?.Any(a => a.DeliveryGate_Id == c.DeliveryGate_Id) ?? false)
                      && c.DeliveryRecord_Status == (int)HTStatusCodeEnum.Stable) ?? 0;
                    if (countGateInProcess > 0)
                        sendStop = true;
                }
                if (!startOn) sendStop = true;

                var plcServices = this.PlcServices;
                var dataPack = new WritePlcTagPkg
                {
                    Plc_DataBlockId = plcDataBlockId,
                    GiaTri = (int)val
                };

                if (packingType == (int)HTPackingTypeEnum.BAO && deviceGate?.PlcDbTypeId == (int)DataBlockType.DbMangxuat_V3)
                {
                    dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiPhieuXuatOn;
                    ret = await plcServices.WritePlcTag(dataPack, AccessToken);

                    if (ret)
                    {
                        dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiStartStop;
                        //stop khi nhập phiếu ở máng có chung máy đóng hoặc kết thúc phiếu hoặc áp đặt không bật startOn
                        dataPack.GiaTri = ((val == 1 && sendStop) || val == 0 ? 0 : 1);
                        _ = !await plcServices.WritePlcTag(dataPack, AccessToken);
                    }
                }
                else if (packingType == (int)HTPackingTypeEnum.BAO && deviceGate?.PlcDbTypeId == (int)DataBlockType.DbMangXuat)
                {
                    dataPack.EnumTag = (int)DbMangXuatEnum.HMI_SoPhieuOn;
                    ret = await plcServices.WritePlcTag(dataPack, AccessToken);

                    if (ret)
                    {
                        dataPack.EnumTag = (int)DbMangXuatEnum.HMI_Start_Stop;
                        //stop khi nhập phiếu ở máng có chung máy đóng hoặc kết thúc phiếu
                        dataPack.GiaTri = ((val == 1 && sendStop) || val == 0 ? 0 : 1);
                        _ = !await plcServices.WritePlcTag(dataPack, AccessToken);
                    }
                }
                else if (packingType == (int)HTPackingTypeEnum.ROI && transportMethod == (int)TransportMethodEnum.BO)//xi rời bộ
                {
                    dataPack.EnumTag = (int)DbCanXaHangEnum.HMI_SoPhieuOn;
                    ret = await plcServices.WritePlcTag(dataPack, AccessToken);
                    //XMR khi kết thúc không stop
                    if (ret && val == 1)
                    {
                        dataPack.EnumTag = (int)DbCanXaHangEnum.HMI_Start_Stop;
                        dataPack.GiaTri = (int)val;//(uint)(autoCreate ? 0 : 1); 
                        _ = !await plcServices.WritePlcTag(dataPack, AccessToken);
                    }

                }
                else if (packingType == (int)HTPackingTypeEnum.ROI && transportMethod == (int)TransportMethodEnum.THUY)//xi rời thủy
                {
                    dataPack.EnumTag = (int)DBCanBangEnum.HMI_SoPhieuOn;
                    ret = await plcServices.WritePlcTag(dataPack, AccessToken);
                    if (ret)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi {possitionDebug}: {ex.Message}");
            }
            //20211001 Đưa lệnh vào hàng đợi
            possitionDebug = $"Đưa lệnh vào hàng đợi (deliveryGateId={deliveryGateId} - Set HMI_SoPhieuOn={val})";
            try
            {
                var plcCmd = this.WaitingPlcCommands?.FirstOrDefault(f => f.DeliveryGateId == deliveryGateId);
                if (plcCmd == null)
                    lock (this.WaitingPlcCommands)
                    {
                        DeliveryUtils.getInstance().WaitingPlcCommands.Add(
                            new WaitingPlcCommandModel
                            {
                                DeliveryGateId = deliveryGateId,
                                DeliveryRecordId = deliveryRecordId,
                                PlcBlockId = plcDataBlockId,
                                Command = val == 1 ? HTPlcCommandEnum.On_HMI_SoPhieu : HTPlcCommandEnum.OFF_HMI_SoPhieu,
                                PackingType = (HTPackingTypeEnum)packingType,
                                LastTimeExec = DateTime.Now,
                                TransportMethod = (TransportMethodEnum)transportMethod
                            });
                    }
                else
                    lock (plcCmd.LockAccess)
                    {
                        plcCmd.DeliveryRecordId = deliveryRecordId;
                        plcCmd.Command = val == 1 ? HTPlcCommandEnum.On_HMI_SoPhieu : HTPlcCommandEnum.OFF_HMI_SoPhieu;
                        plcCmd.Success = false;
                        plcCmd.When = DateTime.Now;
                        plcCmd.LastTimeExec = DateTime.Now;
                        plcCmd.TransportMethod = (TransportMethodEnum)transportMethod;
                    }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi {possitionDebug}: {ex.Message}");
            }

            //20230411 Đưa vào hàng đợi máng test
            if (!isTestGate)
            {
                int action = (int)val;
                DeliveryUtils.getInstance().TestGateListUpdate(deliveryGateId, action == 0 ? -1 : action);
            }
            return;
        }
        public void CheckDurtySensor()
        {
            string wrnMsg = "Cảm biến vị trí phương tiện bị bám bẩn, cần phải vệ sinh!";
            var sensorTracking = XhsServices.GetLastSensorTracking();
            if (sensorTracking?.Count > 0)
                foreach (var sensorEvt in sensorTracking)
                {
                    var gate = this.GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == sensorEvt.DeliveryGateId);
                    if (gate == null || gate?.Transport_Method_Id == (int)TransportMethodEnum.THUY) continue;
                    var deviceGate = this.DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == sensorEvt.DeliveryGateId);

                    //sensor bình thường: lá nội địa (vì xk nhiều ngày), có sự kiện không quá 3h hoặc sensor không bị tác động (sensorEvt.Sensor1=true)
                    if (gate.IsExport >= 1 || (sensorEvt.Sensor1 ?? false) || sensorEvt.CreatedDate.AddHours(3) > DateTime.Now)
                    {
                        if (gate.PositionSensorErrMsg == wrnMsg)
                            lock (gate)
                                gate.PositionSensorErrMsg = "";
                        //reset lỗi
                        if (deviceGate?.SensorErr ?? false) deviceGate.SensorErr = false;

                        continue;
                    }

                    if (gate.PositionSensorErrMsg == wrnMsg) continue;

                    lock (gate)
                        gate.PositionSensorErrMsg = wrnMsg;
                    deviceGate.SensorErr = true;
                }
        }
        /// <summary>
        /// Lựa chọn biển số xe hợp lý nhất của máng xuất khi nhiễu đọc được ở nhiều máng
        /// xe đã xđ biển số và msgh
        /// </summary>
        /// <param name="deliveryGateId"></param>
        /// <returns></returns>
        public List<(string VehicleCode, double CardFrequency)> GetAccurateVehicle(int deliveryGateId)
        {
            List<(string VehicleCode, double CardFrequency)> ret = null;
            try
            {
                var gate = DeliveryGateCardDetects.FirstOrDefault(f => f.DeliveryGate_Id == deliveryGateId);
                if (gate == null) return null;
                //Thẻ đã xác định được xe, không đang xuất ở máng khác
                //tìm tần xuất lớn nhất số lần/tổng thời gian với những thẻ đọc được trên 5 lần
                //Nếu không cách biệt thì thêm tiêu chí phụ: thời gian phát hiện trước
                var detectedCardVehicle = gate.DetectedVehicleCards.Where(w => !string.IsNullOrWhiteSpace(w.VehicleCode)
                //Có số lần đọc được trên 1 lần
                && w.Sum_DetectedNumber > 0
                && w.Sum_DetectedSeconds > 0
                //Xe có phiếu xuất, Cùng chủng loại với máng
                && (gate.WaitingDelivery?.Any(a => a.VehicleCode == w.VehicleCode
                        //&& (gate.CategoryIds?.Contains(a.CategoryId) ?? false)
                        ) ?? false)

                //máng khác chưa gán cho xe, tần xuất thẻ nhỏ hơn 1.2 lần máng hiện tại
                && !DeliveryGateCardDetects.Any(a => a.DeliveryGate_Id != deliveryGateId
                    //đã áp phiếu
                    && (a.DetectedVehicleCards?.Any(d => (d.ManualSelected && d.VehicleCode == w.VehicleCode)
                        //hoặc cùng số thẻ và tần bình quân (lần/s) xuất lớn hơn 20% (1.2), để xác định lớn hơn hẳn, nếu 2 ăng ten đọc được gần giống nhawu thì nhiễu
                        || (d.VehicleCode == w.VehicleCode && d.Sum_DetectedSeconds > 0 && (double)d.Sum_DetectedNumber / d.Sum_DetectedSeconds > (double)w.Sum_DetectedNumber * 1.2 / w.Sum_DetectedSeconds)
                    ) ?? false)
                    )
                )?.ToList();

                if (detectedCardVehicle == null || detectedCardVehicle?.Count <= 0) return ret;// $"{detectedCardVehicle?.Count}";

                ret = new List<(string VehicleCode, double CardFrequency)>();

                detectedCardVehicle.OrderByDescending(o => o.Sum_DetectedNumber / o.Sum_DetectedSeconds)
                    .ToList()
                    .ForEach(fe => ret.Add((fe.VehicleCode, fe.Sum_DetectedNumber / fe.Sum_DetectedSeconds)));
                //.ForEach(fe => ret += $"{fe.VehicleCode}(f={fe.Sum_DetectedNumber / fe.Sum_DetectedSeconds}), ");
            }
            catch { }
            return ret;
        }

        /// <summary>
        /// Ghi log sự kiện thẻ, biển xe khi phát hiện. Dùng test nhiễu máng 2 ăng ten
        /// </summary>
        /// <param name="when"></param>
        /// <param name="deliveryGateId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="readerId"></param>
        /// <param name="vehicleCode"></param>
        public void LogVehicleDetectedToDBLog(DateTime when, int deliveryGateId, long cardNumber, long readerId, string vehicleCode)
        {
            try
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var _ioTDataServices = scope.ServiceProvider.GetRequiredService<IoTDataServices>();
                    (DateTime When, int DeliveryGateId, long CardNumber, long ReaderId, string VehicleCode) ioTLog = (When: when, DeliveryGateId: deliveryGateId, CardNumber: cardNumber, ReaderId: readerId, VehicleCode: vehicleCode);

                    _ioTDataServices.Log_sp($"{JsonConvert.SerializeObject(ioTLog)}", $"{deliveryGateId}.{readerId}.{cardNumber}", LogTypeEnum.INFORMATION_TYPE);
                }
            }
            catch { }
        }

        public bool OpcPermission()
        {
            bool ret = false;
            using (var scope = ServiceProvider.CreateScope())
            {
                var xhsContext = scope.ServiceProvider.GetRequiredService<XhsContext>();
                int oPC_OptionId = xhsContext.Options.FirstOrDefault(f => f.Code == "OPC_REGISTRATION")?.Id ?? 0;
                var lastHistory = xhsContext.OptionsHistories?.Where(w => w.Option_Id == oPC_OptionId)?.OrderByDescending(o => o.Id).FirstOrDefault();
                if (lastHistory == null) return false;
                //Ktra hiệu lực tuỳ chọn
                var checkStatus = lastHistory.Status == (byte)HTStatusCodeEnum.Stable
                    && lastHistory.BeginDate <= DateTime.Now && (lastHistory.EndDate == null || lastHistory.EndDate >= DateTime.Now);
                if (!checkStatus) return false;

                //ktra số lượng đạt giới han?
                var delivery = (from dr in xhsContext.DeliveryRecord
                                join w in xhsContext.WaitDeliveryQueue on dr.WaitDeliveryQueue_Id equals w.Id
                                where dr.BagTypeId == (int)HTBagTypeEnum.XI_ROI
                                && w.SoOrderId == 0 && dr.Status == (int)HTStatusCodeEnum.Complete
                                && !dr.IsTest
                                && dr.CreatedDate >= lastHistory.BeginDate && (dr.CreatedDate < lastHistory.EndDate || lastHistory.EndDate == null)
                                select dr)?.ToList();
                //Chưa xuất
                if (delivery == null || delivery?.Count <= 0) return true;
                //Chưa vượt lượng đk
                double sumQty = (delivery.Sum(s => s.DeliveryQty_Kg) ?? 0.0) / 1000.0;
                if (sumQty < lastHistory.LimitValue) ret = true;
            }
            return ret;
        }

        public string ShippointName(int shippoint_Id)
        {
            string ret = "Không rõ";
            switch (shippoint_Id)
            {
                case 1:
                    ret = "Công ty Xi măng Hoàng Thạch";
                    break;
                case 2:
                    ret = "Công Ty Xi măng tam Điệp";
                    break;
                case 3:
                    ret = "Công ty Xi măng Hạ Long";
                    break;
                case 4:
                    ret = "Trạm Quy Nhơn";
                    break;
                case 5:
                    ret = "Công ty xi măng Bút Sơn";
                    break;
                case 6:
                    ret = "Công ty CP xi măng Vicem Hải Vân";
                    break;
            }
            return ret;
        }
        /// <summary>
        /// So sánh 2 chuỗi gần đúng
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public bool CompareApproximatString(string s1, string s2)
        {
            int i, j, k, loi, saiSo;
            saiSo = (int)Math.Round(s1.Length * 0.3);

            if (s2.Length < (s1.Length - saiSo) || s2.Length > (s1.Length + saiSo)) return false;

            i = j = loi = 0;

            while (i < s1.Length && j < s2.Length)
            {
                if (s1[i] != s2[j])
                {
                    loi++;
                    for (k = 1; k <= saiSo; k++)
                    {
                        if ((i + k < s1.Length) && s1[i + k] == s2[j])
                        {
                            i += k;
                            break;
                        }
                        else if ((j + k < s2.Length) && s1[i] == s2[j + k])
                        {
                            j += k;
                            break;
                        }
                    }
                }
                i++;
                j++;
            }
            loi += s1.Length - i + s2.Length - j;

            if (loi <= saiSo)
                return true;
            else return false;

        }

        /// <summary>
        /// Kiểm tra, xác định trạng thái máng xuất: khi mất sẵn sàng máng xuất thì không thực xuất lệnh đang xuất dở
        /// </summary>
        public void CheckStatusGate()
        {

        }
        /// <summary>
        /// Kiểm tra trọng lượng bì trung bình quá khứ 
        /// </summary>
        /// <param name="vehicleCode"></param>
        /// <param name="secondVehicleCode"></param>
        /// <param name="checkWeght"></param>
        /// <returns></returns>
        public bool CheckTarWeightMean(string vehicleCode, string secondVehicleCode, double checkWeght)
        {
            bool ret = true;
            try
            {
                vehicleCode = vehicleCode.Replace(" ", "").Replace("-", "");
                secondVehicleCode = secondVehicleCode.Replace(" ", "").Replace("-", "");

                using (var scope = ServiceProvider.CreateScope())
                {
                    var xhsContext = scope.ServiceProvider.GetRequiredService<XhsContext>();
                    var lstDelivery = (from d in xhsContext.DeliveryRecord
                                       join w in xhsContext.WaitDeliveryQueue
                                           on d.WaitDeliveryQueue_Id equals w.Id
                                       join v in xhsContext.VehicleQueue
                                           on w.VehicleQueue_ID equals v.Id
                                       where d.DeliveryQty_Kg > 0
                                           && d.Status == (int)HTStatusCodeEnum.Complete
                                           && !d.IsTest
                                           && d.BagTypeId == (int)HTBagTypeEnum.XI_ROI
                                           && d.CreatedDate > DateTime.Now.AddYears(2)
                                           && v.VehicleCode.Replace(" ", "") == vehicleCode
                                           && v.SecondVehicleCode.Replace(" ", "") == secondVehicleCode
                                       select new { TarWeight = d.Weight_1 ?? 0.0 })?.ToList();
                    if (lstDelivery == null) return true;

                    var meanWeight = lstDelivery.Average(a => a.TarWeight);
                    var lstFilterNoiseWeight = lstDelivery.Where(w => w.TarWeight > meanWeight * 0.6 && w.TarWeight < meanWeight * 1.4)?.ToList();
                    //Nếu dữ liệu phân tán thì bỏ qua, coi là đúng
                    if (lstFilterNoiseWeight == null) return true;
                    //Tính lại TB với dữ liệu đã loại nhiễu (phân tán)
                    var newMeanWeight = lstFilterNoiseWeight.Average(a => a.TarWeight);
                    //Bì được coi là tốt nếu lệch dưới 5% so với bình quân
                    if (Math.Abs(newMeanWeight - checkWeght) / newMeanWeight > 0.05) ret = false;
                }
            }
            catch { }
            return ret;
        }

        #region For test gates
        public void TestGateListUpdate(int mainDeliveryGateId, int action)
        {
            try
            {
                //Tìm máng test trên cung máng chính mainDeliveryGateId
                var tGate = DeliveryGateCardDetects.FirstOrDefault(f => f.TestOnGateId == mainDeliveryGateId);
                if (tGate == null) return;

                var testGate = TestGateList.FirstOrDefault(f => f.DeliveryGateId == tGate.DeliveryGate_Id);
                if (testGate == null)
                    TestGateList.Add(new TestGate { DeliveryGateId = tGate.DeliveryGate_Id, Action = action });
                else
                {
                    testGate.Action = action;
                    testGate.When = DateTime.Now;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Thủ tục set giá trị xuống Plc của máng test
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<bool> TestGateAction(string accessToken = "")
        {
            foreach (var tgate in TestGateList)
            {
                try
                {
                    if (tgate.Action != 1 && tgate.Action != -1) continue;
                    var gateModel = GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == tgate.DeliveryGateId);
                    if (gateModel == null)
                    {
                        tgate.Action = 0;
                        continue;
                    }
                    using XhsContext xhsCt = XhsContext;
                    //Lấy thông tin phiếu đã ghi xuống DB theo tạo phiếu máng chính
                    var deliveryRecord = xhsCt.DeliveryRecord.Where(w => w.DeliveryGateId == tgate.DeliveryGateId)
                        ?.OrderByDescending(o => o.Id)?.FirstOrDefault();
                    //chưa tạo được db thì chưa thực hiện
                    if (deliveryRecord == null) continue;

                    var deviceGate = this.DeliveryGateCardDetects?.FirstOrDefault(f => f.DeliveryGate_Id == tgate.DeliveryGateId) ?? new();
                    if (deviceGate.DeliveryGate_Id == 0) continue;

                    //Kết thúc
                    if (tgate.Action == -1)
                    {
                        tgate.Msg = TestGateFinishDr(tgate.DeliveryGateId);
                        if (string.IsNullOrWhiteSpace(tgate.Msg))
                        {
                            await ChangePlc_HMI_SoPhieuOn(tgate.DeliveryGateId, deviceGate.PlcDataBlockId, gateModel.Gate_PackingType_Id, deliveryRecord.Id, 0, gateModel.Transport_Method_Id.Value, isTestGate: true);
                            tgate.Action = 0;
                        }
                    }
                    //Thêm mới khi: phiếu đã ghi DB
                    else if (tgate.Action == 1 && (deliveryRecord == null || deliveryRecord.Status == (int)HTStatusCodeEnum.Stable))
                    {
                        bool setOnGate = false;

                        //Gán giá trị xuống Plc
                        if (deviceGate.PlcDbTypeId == (int)DataBlockType.DbMangxuat_V3)
                        {
                            var dataPack = new WritePlcTagPkg
                            {
                                Plc_DataBlockId = deviceGate.PlcDataBlockId,
                                EnumTag = (int)DbMangXuatEnum_V3.Input_HmiLuongBot,
                                GiaTri = gateModel.CountedQty
                            };

                            if (!await PlcServices.WritePlcTag(dataPack, accessToken))
                                continue;
                            Task.Delay(100);

                            dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HMILuongDat;
                            dataPack.GiaTri = deliveryRecord.SetpointQty_Bags ?? 0;
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;
                            Task.Delay(100);

                            dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiSoPhieu;
                            dataPack.GiaTri = deliveryRecord.Id;
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;

                            setOnGate = true;
                        }
                        else if (deviceGate.PlcDbTypeId == (int)DataBlockType.DbCanbang)
                        {
                            var dataPack = new WritePlcTagPkg
                            {
                                Plc_DataBlockId = deviceGate.PlcDataBlockId,
                                EnumTag = (int)DBCanBangEnum.HMI_So_phieu,
                                GiaTri = deliveryRecord.Id
                            };
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;
                            Task.Delay(100);
                            dataPack.EnumTag = (int)DBCanBangEnum.HMI_Luong_Dat;
                            dataPack.GiaTri = (int)(deliveryRecord.SetpointQty_Kg ?? 0);
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;
                            Task.Delay(100);
                            dataPack.EnumTag = (int)DbMangXuatEnum_V3.Input_HmiSoPhieu;
                            dataPack.GiaTri = deliveryRecord.Id;
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;

                            setOnGate = true;
                        }
                        else if (deviceGate.PlcDbTypeId == (int)DataBlockType.DbCanXaHang)
                        {
                            var dataPack = new WritePlcTagPkg
                            {
                                Plc_DataBlockId = deviceGate.PlcDataBlockId,
                                EnumTag = (int)DbCanXaHangEnum.HMI_So_phieu,
                                GiaTri = deliveryRecord.Id
                            };
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;
                            Task.Delay(100);
                            dataPack.EnumTag = (int)DbCanXaHangEnum.HMI_Luong_Dat;
                            dataPack.GiaTri = (int)(deliveryRecord.SetpointQty_Kg ?? 0);
                            if (!await PlcServices.WritePlcTag(dataPack, accessToken)) continue;

                            setOnGate = true;
                        }

                        if (setOnGate)
                        {
                            await ChangePlc_HMI_SoPhieuOn(tgate.DeliveryGateId, deviceGate.PlcDataBlockId, gateModel.Gate_PackingType_Id, deliveryRecord.Id, 1, gateModel.Transport_Method_Id.Value, isTestGate: true);
                            tgate.Action = 0;
                            tgate.Msg = "";
                        }
                    }
                }
                catch (Exception ex) { tgate.Msg = ex.Message; }
            }
            return true;
        }
        /// <summary>
        /// Kết thúc phiếu test, để ghi DB
        /// </summary>
        public string TestGateFinishDr(int testDeliveryGateId)
        {
            try
            {
                var OperateViewModel = GateOperationsList?.FirstOrDefault(f => f.DeliveryGate_Id == testDeliveryGateId);
                //máng chính thì bỏ qua
                if (OperateViewModel == null || OperateViewModel?.TestOnGateId == 0) return "Không tồn tại máng trong ds GateOperationsList";
                var deliveryRecord = XhsContext.DeliveryRecord.FirstOrDefault(f => f.Id == OperateViewModel.DeliveryRecord_Id);
                if (deliveryRecord == null || deliveryRecord?.Status != (int)HTStatusCodeEnum.Stable) return "Không tồn tại phiếu xuất trong DB"; ;

                double curDeliveryQty_Kg = 0;
                if (OperateViewModel.PackingType_Id == (int)HTPackingTypeEnum.BAO)
                {
                    int soBao = OperateViewModel.CurrentCountQty;
                    deliveryRecord.DeliveryQty_Bags = soBao;
                    curDeliveryQty_Kg = soBao * OperateViewModel.BagWeight;
                }
                else if (OperateViewModel.PackingType_Id == (int)HTPackingTypeEnum.ROI)
                {
                    //deliveryRecord.Weight_2 = (int)(Math.Round((double)OperateViewModel.CurrentCountQty / 1000.0, 2) * 1000);
                    deliveryRecord.Weight_2 = DeliveryUtils.getInstance().LamTronXMR(OperateViewModel.CurrentCountQty, 1);
                    curDeliveryQty_Kg = deliveryRecord.Weight_2.Value - deliveryRecord.Weight_1.Value;// OperateViewModel.DBCanXaHang.HMI_Luong_Bi;
                    curDeliveryQty_Kg = curDeliveryQty_Kg > 150 ? curDeliveryQty_Kg : 0;
                }

                deliveryRecord.DeliveryQty_Kg = curDeliveryQty_Kg;
                deliveryRecord.EndDate = DateTime.Now;
                deliveryRecord.Status = (byte)HTStatusCodeEnum.Complete;
                deliveryRecord.DeliveryDate = DeliveryUtils.getInstance().CalculateDeliveryDate(DateTime.Now);
                deliveryRecord.ModifiedBy = 0;
                deliveryRecord.ModifiedDate = DateTime.Now;
                deliveryRecord.EndCounterQty = OperateViewModel.CurrentCountQty;

                string out_error_message = "";
                bool dSuccess = XhsServices.p_DeliveryRecord_Update(out _, out out_error_message, null, deliveryRecord);
                return out_error_message;
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }
        #endregion

        /// <summary>
        /// Ktra xem có xe khác vào máng không
        /// </summary>
        /// <param name="lastGateData"></param>
        /// <param name="gateWithCard"></param>
        /// <returns></returns>
        public DetectedVehicleCard? GetOtherVihicleArrived(GateOperationViewModel lastGateData, DeliveryGateCardDetect gateWithCard)
        {
            //ktra có xe khác đang có đăng ký, phát hiện hơn 1 lần vào sau không
            var otherVehicle = gateWithCard.DetectedVehicleCards
                ?.Where(w => !w.ManualSelected
                    && w.Sum_DetectedNumber > 1
                    && !string.IsNullOrWhiteSpace(w.VehicleCode)
                    && w.VehicleCode != lastGateData.Vehicle_Code)
                ?.OrderByDescending(o => o.When)?.FirstOrDefault();

            //sk xe hiện tại
            var curCardVehicle = gateWithCard.DetectedVehicleCards
                ?.Where(w => w.VehicleCode == lastGateData.Vehicle_Code)
                ?.OrderByDescending(o => o.When)?.FirstOrDefault();

            //xe khác vào sau?: xe mới thấy sau xe đang xuất ít nhất 5s và xe hiện tại không phát hiện quá 30s
            var checkOtherInAfter = otherVehicle != null && curCardVehicle != null
                //phát hiện vào sau xe đang xuất ít nhất 2s
                && otherVehicle.When > curCardVehicle.When.AddSeconds(2);

            return checkOtherInAfter ? otherVehicle : (DetectedVehicleCard?)null;
        }
        /// <summary>
        /// Lấy tổng số lượng đã xuất (Kg) của xe trong khoảng thời gian (số phút) trở lại đây
        /// <0 : không xác định được
        /// </summary>
        /// <param name="vehicleCode">Biển số xe</param>
        /// <param name="fromMinutes"></param>
        /// <returns></returns>
        public double GetDeliveriedQtyKgOfVehicleTrip(string vehicleCode, int fromMinutes)
        {
            double ret = -1;
            try
            {
                using var scope = ServiceProvider?.CreateScope();
                if (scope == null) return -1;

                var xhs = scope.ServiceProvider.GetRequiredService<XhsContext>();
                var deliveries = (from d in xhs.DeliveryRecord
                                  join w in xhs.WaitDeliveryQueue on d.WaitDeliveryQueue_Id equals w.Id
                                  join v in xhs.VehicleQueue on w.VehicleQueue_ID equals v.Id
                                  where d.Status == (byte)HTStatusCodeEnum.Complete
                                  && d.CreatedDate.AddMinutes(fromMinutes) > DateTime.Now
                                  && d.DeliveryQty_Kg > 0
                                  && v.VehicleCode == vehicleCode
                                  select new { d.DeliveryQty_Kg })?.ToList();

                if (deliveries == null) return 0;

                ret = deliveries.Sum(s => s.DeliveryQty_Kg ?? 0);
            }
            catch
            {
            }
            return ret;
        }
        /// <summary>
        /// Làm tròn số lượng cân, vói XMR làm tròn đến hàng chục thì soChuSo=1 (cắt 1 chữ số đơn vị)
        /// </summary>
        /// <param name="soLamTron">Số nguyên cần làm tròn</param>
        /// <param name="soChuSo">Số chữ số cần làm tròn>0</param>
        /// <returns></returns>
        public int LamTronXMR(int soLamTron, int soChuSo)
        {
            if (soChuSo < 0) soChuSo = 0;
            if (soChuSo == 0) return soLamTron;

            var divNum = Math.Pow(10, soChuSo);
            //
            //làm tròn lên
            var c = Math.Ceiling(soLamTron / divNum);
            var x = c * divNum;

            return (int)x;
        }
        /// <summary>
        /// Chuyển đổi từ CategoryId (theo ERP) sang dữ liệu byte chủng loại trong Plc
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public byte CategoryId_To_PlcDbChungLoai(int categoryId)
        {
            byte chungloai = 0;
            switch (categoryId)
            {
                case (int)HTCategory.PCB30:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.PCB30;
                    break;
                case (int)HTCategory.PCB40:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.PCB40;
                    break;
                case (int)HTCategory.MC25:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.MC25;
                    break;
                case (int)HTCategory.ROI_CN:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.ROI_CN;
                    break;
                case (int)HTCategory.ROI_DD:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.ROI_DD;
                    break;
                case (int)HTCategory.ROI_KHAC:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.ROI_KHAC;
                    break;
                default:
                    chungloai = (byte)HtPlcDbChungLoaiEnum.LOAI_KHAC;
                    break;
            }
            return chungloai;
        }
    }
}
