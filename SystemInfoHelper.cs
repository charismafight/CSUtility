public static class SystemInfoHelper
    {
        public static string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码
                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
        }

        public static string GetMacAddress()
        {
            return GetNetWorkInfo("MacAddress");
        }

        public static string GetIPAddress()
        {
            return GetNetWorkInfo("IpAddress");
        }

        private static string GetNetWorkInfo(string attr)
        {
            try
            {
                //获取IP地址
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        var systemProperty = mo.Properties[attr].Value;
                        //ip的属性是一个包含ipv4和ipv6的数组，第一个是ipv4地址
                        return systemProperty.GetType().Equals(typeof(String[]))
                            ? ((Array)systemProperty).GetValue(0).ToString()
                            : systemProperty.ToString();
                    }
                }

                return "unknow";
            }
            catch
            {
                return "unknow";
            }
        }

        /// <summary>
        /// 调用API获取MAC地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>MAC地址</returns>
        public static string GetMac(string ip)
        {
            IPAddress _Address;
            if (!IPAddress.TryParse(ip, out _Address)) return "";
            uint DestIP = System.BitConverter.ToUInt32(_Address.GetAddressBytes(), 0);
            ulong pMacAddr = 0;
            uint PhyAddrLen = 6;
            uint error_code = SendARP(DestIP, 0, ref pMacAddr, ref PhyAddrLen);
            byte[] _Bytes1 = BitConverter.GetBytes(pMacAddr);
            return BitConverter.ToString(_Bytes1, 0, 6);
        }

        /// <summary>
        /// 使用ARP获取MAC地址
        /// </summary>
        /// <param name="DestIP">目标IP</param>
        /// <param name="SrcIP">0</param>
        /// <param name="pMacAddr">两个uint 都是255</param>
        /// <param name="PhyAddrLen">长度6</param>
        /// <returns>返回错误信息</returns>
        [DllImport("Iphlpapi.dll")]
        public static extern uint SendARP(uint DestIP, uint SrcIP, ref ulong pMacAddr, ref uint PhyAddrLen);
    }