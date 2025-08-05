using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using MEC;

namespace PlayerBadge
{
    public class BadgeManager
    {
        private List<BadgeData> _badges = new List<BadgeData>();

        private HashSet<Player> _rainbowPlayers = new HashSet<Player>();

        private CoroutineHandle _rainbowCoroutine;

        private readonly string[] _availableColors = { "red", "yellow", "cyan", "green", "aqua", "pink", "white", "orange" };

        private int _currentColorIndex = 0;

        public void LoadBadges()
        {
            try
            {
                var configPath = PlayerBadge.Instance.Config.ConfigFilePath;

                var directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(configPath))
                {
                    CreateExampleConfigFile(configPath);
                }

                var lines = File.ReadAllLines(configPath);
                _badges.Clear();

                foreach (var line in lines)
                {
                    var badge = BadgeData.ParseFromConfigLine(line);
                    if (badge != null)
                    {
                        _badges.Add(badge);
                        if (PlayerBadge.Instance.Config.Debug)
                        {
                            Log.Debug($"加载称号: {badge}");
                        }
                    }
                }

                Log.Debug($"成功加载 {_badges.Count} 个玩家称号配置");

                StartRainbowBadges();
            }
            catch (Exception ex)
            {
                Log.Error($"加载称号配置时发生错误: {ex.Message}");
            }
        }

        private void CreateExampleConfigFile(string configPath)
        {
            var exampleContent = @"# PlayerBadge 配置文件
# 格式: 玩家ID@平台:颜色:称号内容
# 支持的平台: steam, discord
# 支持的颜色: red, yellow, cyan, green, aqua, pink, white, orange, rainbow
# 示例:
# 76561198000000000@steam:red:管理员
# 123456789@discord:rainbow:VIP玩家
# 76561198111111111@steam:green:测试员

# 在此添加您的玩家称号配置
";
            File.WriteAllText(configPath, exampleContent);
            Log.Info($"已创建示例配置文件: {configPath}");
        }

        public void ApplyBadgeToPlayer(Player player)
        {
            if (player == null)
                return;

            try
            {
                var badge = GetBadgeForPlayer(player);
                if (badge != null)
                {
                    if (badge.IsRainbow)
                    {
                        _rainbowPlayers.Add(player);
                        player.RankName = badge.Text;
                        player.RankColor = _availableColors[_currentColorIndex];
                        
                        if (PlayerBadge.Instance.Config.Debug)
                        {
                            Log.Debug($"为玩家 {player.Nickname} 应用彩色称号: {badge.Text}");
                        }
                    }
                    else
                    {
                        player.RankName = badge.Text;
                        player.RankColor = badge.Color;
                        
                        if (PlayerBadge.Instance.Config.Debug)
                        {
                            Log.Debug($"为玩家 {player.Nickname} 应用称号: {badge.Text} ({badge.Color})");
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Error($"为玩家 {player?.Nickname} 应用称号时发生错误: {ex.Message}");
            }
        }

        private BadgeData GetBadgeForPlayer(Player player)
        {
            return _badges.FirstOrDefault(badge => badge.MatchesPlayer(player));
        }

        private void StartRainbowBadges()
        {
            if (_rainbowCoroutine.IsRunning)
                Timing.KillCoroutines(_rainbowCoroutine);

            _rainbowCoroutine = Timing.RunCoroutine(RainbowBadgeCoroutine());
        }

        public void StopRainbowBadges()
        {
            if (_rainbowCoroutine.IsRunning)
                Timing.KillCoroutines(_rainbowCoroutine);
            
            _rainbowPlayers.Clear();
        }

        private IEnumerator<float> RainbowBadgeCoroutine()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(PlayerBadge.Instance.Config.RainbowInterval);

                if (_rainbowPlayers.Count > 0)
                {
                    _currentColorIndex = (_currentColorIndex + 1) % _availableColors.Length;
                    var currentColor = _availableColors[_currentColorIndex];

                    var playersToRemove = new List<Player>();
                    foreach (var player in _rainbowPlayers)
                    {
                        if (player == null || !player.IsConnected)
                        {
                            playersToRemove.Add(player);
                            continue;
                        }

                        try
                        {
                            player.RankColor = currentColor;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"更新玩家 {player.Nickname} 彩色称号时发生错误: {ex.Message}");
                            playersToRemove.Add(player);
                        }
                    }

                    foreach (var player in playersToRemove)
                    {
                        _rainbowPlayers.Remove(player);
                    }
                }
            }
        }

        public void ReloadConfig()
        {
            Log.Debug("正在重新加载称号配置...");
            StopRainbowBadges();
            LoadBadges();
        }

        public void RemoveRainbowPlayer(Player player)
        {
            _rainbowPlayers.Remove(player);
        }
    }
}
