using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace NAVI_System
{
    public class COM // класс передачи по последовательному порту
    {

        SerialPort mySerialPort = new SerialPort();
        string name;

        public delegate object ReadDelegate(CancellationToken cancelTok);
        public delegate bool DateProcessDelegate(Queue<object> queue);
        public ReadDelegate read;
        public DateProcessDelegate date_process;

        public int status; //0-все хорошо, 1 - исключение открытия порта, 2 - исключение чтения
        public COM(COM_Date com)
        {
            name = com.DeviceName;
            mySerialPort.PortName = com.PortName;
            mySerialPort.BaudRate = (int)com.BaudRate;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = (int)com.DataBits;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.ReadTimeout = 300;

            switch (com.DeviceName) // в зависимости от устройства задаются функции чтения дланных и их обработки
            {
                case "DVL":
                    read = read_DVL;
                    date_process = date_process_DVL;
                    break;
                case "SonarDyne":
                    read = read_DVL;
                    date_process = date_process_DVL;
                    break;
                case "Test":
                    read = read_Test;
                    date_process = date_process_Test;
                    break;
                default:
                    break;
            }
        }
        public bool open_port()
        {
            if (mySerialPort.IsOpen) { return true; }
            else
            {
                try
                {
                    mySerialPort.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool close_port()
        {

            if (mySerialPort.IsOpen)
            {
                mySerialPort.Close();
                return true;
            }
            else
            {
                return false;
            }

        }
        public object read_DVL(CancellationToken cancelTok)
        {
            byte[] buf = new byte[88];
            int count = 88;
            for (int i = 0; i < count; i++)
            {
                if (cancelTok.IsCancellationRequested) cancelTok.ThrowIfCancellationRequested();

                try
                {
                    buf[i] = (byte)mySerialPort.ReadByte();
                }
                catch (Exception) { }

                if (i > 0 && buf[i - 1] == 125)
                {
                    if (buf[i] == 1)
                    {
                        buf[0] = buf[i - 1];
                        buf[1] = buf[i];
                        i = 1;
                        count = 88;
                    }
                    else if (buf[i] == 0)
                    {
                        buf[0] = buf[i - 1];
                        buf[1] = buf[i];
                        i = 1;
                        count = 47;
                    }
                }
            }
            if (buf.Length > 46) return buf;
            else return null;
        }
        public object read_Test(CancellationToken cancelTok)
        {
            byte[] buf = new byte[4];
            byte[] date = new byte[4];
            //byte a = ;
            int count = 4;
            for (int i = 0; i < count; i++)
            {
                if (cancelTok.IsCancellationRequested)
                {
                    cancelTok.ThrowIfCancellationRequested();
                    return null;
                }
                if (i > 0 && buf[i-1] == 12)
                {
                    i = 0;
                    //count=4;
                }
                    
                try
                {
                    buf[i] = (byte)mySerialPort.ReadByte();
                }
                catch (Exception) { }
                
            }
            return buf;
            //else return null;
        }

        public bool date_process_Test(Queue<object> queue)
        {
            byte[] date;
            lock (queue) date = (byte[])queue.Dequeue();

            if (date == null) return false;

            byte[] date_tmp = new byte[4];
            Navigation_Date.DVL.Heading = BitConverter.ToInt32(date,0);
            Action update = () => // асинхронно запуцскаем обновление интенрфейса
            {
                 Objects_Links.M_Window?.Dispatcher.Invoke(() => { Objects_Links.M_Window.update_DVL(); });
            };
            update();
            return true;
        }

        public bool date_process_DVL(Queue<object> queue)
        {
            byte[] date;
            byte[] date_tmp = new byte[88];
            lock (queue) date = (byte[])queue.Dequeue();
            if (date == null) return false;

            bool state = false;
            short crc = new short();
            int crc1 = new int();

            for (int i = 0; i < date.Length; i++)
            {
                if (date[i] == 125)
                {

                    if (date[i + 1] == 1 && date.Length - i >= 88)
                    {
                        for (int i1 = 0; i1 < 88; i1++)
                        {
                            date_tmp[i1] = date[i + i1];
                            if (i1 != 86 && i1 != 87)
                            {
                                crc1 += date_tmp[i1];
                                crc = (short)crc1;
                            }

                        }
                        if (crc == BitConverter.ToInt16(date_tmp, 86))
                        {
                            state = true;
                        }
                        break;
                    }
                    else if (date[i + 1] == 0 && date.Length - i >= 47)
                    {
                        for (int i2 = 0; i2 < 47; i2++)
                        {
                            date_tmp[i2] = date[i + i2];
                            if (i2 != 45 && i2 != 46)
                            {
                                crc1 += date_tmp[i2];
                                crc = (short)crc1;
                            }

                        }
                        if (crc == BitConverter.ToInt16(date_tmp, 45))
                        {
                            state = true;
                        }
                        break;
                    }
                }
                state = false;
            }
            if (state == false) return false; //устройство не читается
            else  // устройство прочитано (формируем данные)
            {
                Navigation_Date.DVL.Heading = BitConverter.ToInt16(date_tmp, 52);
                Navigation_Date.DVL.X_Vel = BitConverter.ToInt16(date_tmp, 5); 
                Navigation_Date.DVL.Y_Vel = BitConverter.ToInt16(date_tmp, 7);
                Navigation_Date.DVL.Z_Vel = BitConverter.ToInt16(date_tmp, 9);
                Navigation_Date.DVL.Pitch = BitConverter.ToInt16(date_tmp, 48);
                Navigation_Date.DVL.Roll = BitConverter.ToInt16(date_tmp, 50);
                Navigation_Date.DVL.X_Ref_Vel = BitConverter.ToInt16(date_tmp, 22);
                Navigation_Date.DVL.Y_Ref_Vel = BitConverter.ToInt16(date_tmp, 24);
                Navigation_Date.DVL.Z_Ref_Vel = BitConverter.ToInt16(date_tmp, 26);
                Navigation_Date.DVL.Depth = BitConverter.ToInt16(date_tmp, 46);
                Navigation_Date.DVL.Temperature = BitConverter.ToInt16(date_tmp, 43);
                //Navigation_Date.DVL.Measured_Points; ***************************************обязательно добавить!!!!!

                Action update = async () => // асинхронно запуцскаем обновление интенрфейса
                {
                    await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => { Objects_Links.M_Window.update_DVL(); });
                };
                update();
                state = false;
                return true;
            }



        }
    }
}
