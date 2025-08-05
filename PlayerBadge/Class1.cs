using System;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace PlayerBadge
{
    public class PlayerBadge : Plugin<Config>
    {
        public static PlayerBadge Instance { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public BadgeManager BadgeManager { get; private set; }

        public override string Name => "PlayerBadge";

        public override string Author => "kldhsh123";

        public override Version Version => new Version(1, 0, 0);

        public override void OnEnabled()
        {
            Instance = this;

            BadgeManager = new BadgeManager();

            EventHandlers = new EventHandlers();

            Exiled.Events.Handlers.Player.Verified += EventHandlers.OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnPlayerLeft;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;

            Log.Info("PlayerBadge插件已启用！");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnPlayerLeft;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;

            BadgeManager?.StopRainbowBadges();

            EventHandlers = null;
            BadgeManager = null;
            Instance = null;

            Log.Info("PlayerBadge插件已禁用！");
            base.OnDisabled();
        }
    }
}
