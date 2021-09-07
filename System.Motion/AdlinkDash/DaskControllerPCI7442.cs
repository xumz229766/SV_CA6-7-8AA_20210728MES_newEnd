using System;
using System.Collections.Generic;
using Motion.Interfaces;

namespace Motion.AdlinkDash
{
    /// <summary>
    ///     凌华科技 7230 IO 卡控制器。
    /// </summary>
    public class DaskControllerPCI7442 : Automatic, ISwitchControllerPCI7442, INeedInitialization, IDisposable
    {
        private readonly byte[] _cardNos;
        private readonly Dictionary<int, short> _devices;
        private bool _disposed;
        public ushort typeName { get; set; }
        public bool _InitSucess;
        public DaskControllerPCI7442(params byte[] cardNos)
        {
            _cardNos = cardNos;
            _devices = new Dictionary<int, short>();
        }

        #region Implementation of INeedInitialization

        public void Initialize()
        {
            foreach (var cardNo in _cardNos)
            {
                var device = DASK.Register_Card(typeName, cardNo);

                if (device < 0)
                {
                    _InitSucess = false;
                    switch (device)
                    {
                        case DASK.ErrorTooManyCardRegistered:
                            throw new DaskException("32 cards have been registered.");
                        case DASK.ErrorUnknownCardType:
                            throw new DaskException(string.Format("The CardType argument {0} is not valid.", DASK.PCI_7432));
                        case DASK.ErrorOpenDriverFailed:
                            throw new DaskException("Failed to open the device driver.");
                        case DASK.ErrorOpenEventFailed:
                            throw new DaskException("Open event failed in device driver.");
                        default:
                            throw new DaskException("Unknown error.");
                    }
                   
                }
                _InitSucess = true;
                _devices.Add(cardNo, device);
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _InitSucess = false;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DaskControllerPCI7442()
        {
            _InitSucess = false;
            Dispose(false);
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            _InitSucess = false;
            if (_disposed)
                return;
            //if (disposing)
            //{
            //    //清理托管资源
            //    if (managedResource != null)
            //    {
            //        managedResource.Dispose();
            //        managedResource = null;
            //    }
            //}

            // 清理非托管资源
            foreach (var device in _devices.Values)
            {
                var ret = DASK.Release_Card((ushort)device);
                if (ret != DASK.NoError)
                {
                    throw new DaskException("Unknown error.");
                }
            }
            _disposed = true;
        }

        #endregion

        #region Implementation of ISwitchController

        public bool Read(IoPointPCI7442 ioPoint)
        {
            if (!_InitSucess) return false;
            var device = _devices[ioPoint.BoardNo];
            ushort value = 0;
            var ret = 0;
            if (ioPoint.IoMode != IoModes.Signal)
            {
                ret = DASK.DI_ReadLine((ushort)device, (ushort)ioPoint.ReadProtNo, (ushort)ioPoint.PortNo, out value);
            }
            else
            {
                ret = DASK.DO_ReadLine((ushort)device, (ushort)ioPoint.ReadProtNo, (ushort)ioPoint.PortNo, out value);
            }
            if (ret != DASK.NoError)
            {
                switch (ret)
                {
                    case DASK.ErrorInvalidCardNumber:
                        throw new DaskException(string.Format("The CardNumber argument {0} is out of range (larger than 31).", device));
                    case DASK.ErrorCardNotRegistered:
                        throw new DaskException(string.Format("No card registered as {0} CardNumber.", device));
                    case DASK.ErrorFuncNotSupport:
                        throw new DaskException(string.Format("The {0} function called is not supported by this type of card.", "DO_WriteLine"));
                    case DASK.ErrorInvalidIoChannel:
                        throw new DaskException(string.Format("The specified Channel or Port argument {0} is out of range.", ioPoint.PortNo));
                    default:
                        throw new DaskException("Unknown error.");
                }
            }
            return value > 0;
        }

        public void Write(IoPointPCI7442 ioPoint, bool value)
        {
            if (!_InitSucess) return ;
            var device = _devices[ioPoint.BoardNo];
            var ret = DASK.DO_WriteLine((ushort)device, 0, (ushort)ioPoint.PortNo, (ushort)(value ? 1 : 0));
            if (ret != DASK.NoError)
            {
                switch (ret)
                {
                    case DASK.ErrorInvalidCardNumber:
                        throw new DaskException(string.Format("The CardNumber argument {0} is out of range (larger than 31).", device));
                    case DASK.ErrorCardNotRegistered:
                        throw new DaskException(string.Format("No card registered as {0} CardNumber.", device));
                    case DASK.ErrorFuncNotSupport:
                        throw new DaskException(string.Format("The {0} function called is not supported by this type of card.", "DO_WriteLine"));
                    case DASK.ErrorInvalidIoChannel:
                        throw new DaskException(string.Format("The specified Channel or Port argument {0} is out of range.", ioPoint.PortNo));
                    default:
                        throw new DaskException("Unknown error.");
                }
            }
        }

        #endregion

        /// <summary>
        ///     读端口数据
        /// </summary>
        /// <param name="boardNo"></param>
        /// <param name="portNo"></param>
        /// <returns></returns>
        public uint ReadPort(int boardNo, int portNo = 0)
        {
            if (!_InitSucess) return 0;
            var device = _devices[boardNo];

            uint value;
            var ret = DASK.DI_ReadPort((ushort)device, (ushort)portNo, out value);
            if (ret != DASK.NoError)
            {
                switch (ret)
                {
                    case DASK.ErrorInvalidCardNumber:
                        throw new DaskException(string.Format("The CardNumber argument {0} is out of range (larger than 31).", boardNo));
                    case DASK.ErrorCardNotRegistered:
                        throw new DaskException(string.Format("No card registered as {0} CardNumber.", boardNo));
                    case DASK.ErrorFuncNotSupport:
                        throw new DaskException(string.Format("The {0} function called is not supported by this type of card.", "DI_ReadPort"));
                    default:
                        throw new DaskException("Unknown error.");
                }
            }
            return value;
        }
    }
}