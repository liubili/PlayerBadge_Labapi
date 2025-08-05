using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace PlayerBadge
{
    public class EventHandlers
    {
        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            try
            {
                if (ev.Player == null)
                    return;

                if (PlayerBadge.Instance.Config.Debug)
                {
                    Log.Debug($"玩家 {ev.Player.Nickname} ({ev.Player.UserId}) 已验证，正在检查称号配置...");
                }

                PlayerBadge.Instance.BadgeManager.ApplyBadgeToPlayer(ev.Player);
            }
            catch (System.Exception ex)
            {
                Log.Error($"处理玩家验证事件时发生错误: {ex.Message}");
            }
        }

        public void OnPlayerLeft(LeftEventArgs ev)
        {
            try
            {
                if (ev.Player == null)
                    return;

                PlayerBadge.Instance.BadgeManager?.RemoveRainbowPlayer(ev.Player);

                if (PlayerBadge.Instance.Config.Debug)
                {
                    Log.Debug($"玩家 {ev.Player.Nickname} 已离开，已清理相关数据");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"处理玩家离开事件时发生错误: {ex.Message}");
            }
        }

        public void OnWaitingForPlayers()
        {
            try
            {
                if (PlayerBadge.Instance.Config.Debug)
                {
                    Log.Debug("回合重启，正在重新加载称号配置...");
                }

                PlayerBadge.Instance.BadgeManager.LoadBadges();
            }
            catch (System.Exception ex)
            {
                Log.Error($"处理回合重启事件时发生错误: {ex.Message}");
            }
        }
    }
}
