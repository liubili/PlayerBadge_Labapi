using System.ComponentModel;
using Exiled.API.Interfaces;

namespace PlayerBadge
{
    public class Config : IConfig
    {
        [Description("是否启用PlayerBadge插件")]
        public bool IsEnabled { get; set; } = true;

        [Description("是否启用调试日志输出")]
        public bool Debug { get; set; } = false;

        [Description("称号配置文件的完整路径")]
        public string ConfigFilePath { get; set; } = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "EXILED", "Configs", "PlayerBadge.txt");

        [Description("彩色称号颜色切换间隔时间（秒）")]
        public float RainbowInterval { get; set; } = 0.6f;


    }
}
