using System;

namespace PlayerBadge
{
    public class BadgeData
    {
        public string PlayerId { get; set; }

        public string Platform { get; set; }

        public string Color { get; set; }

        public string Text { get; set; }

        public bool IsRainbow { get; set; }

        public BadgeData(string playerId, string platform, string color, string text)
        {
            PlayerId = playerId;
            Platform = platform;
            Color = color;
            Text = text;
            IsRainbow = color.Equals("rainbow", StringComparison.OrdinalIgnoreCase);
        }

        public static BadgeData ParseFromConfigLine(string configLine)
        {
            if (string.IsNullOrWhiteSpace(configLine) || configLine.StartsWith("#"))
                return null;

            try
            {
                var parts = configLine.Split(':');
                if (parts.Length < 3)
                    return null;

                var idPlatformPart = parts[0];
                var atIndex = idPlatformPart.LastIndexOf('@');
                if (atIndex == -1)
                    return null;

                var playerId = idPlatformPart.Substring(0, atIndex);
                var platform = idPlatformPart.Substring(atIndex + 1);

                var color = parts[1];

                var text = string.Join(":", parts, 2, parts.Length - 2);

                return new BadgeData(playerId, platform, color, text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool MatchesPlayer(Exiled.API.Features.Player player)
        {
            if (player == null)
                return false;

            switch (Platform.ToLower())
            {
                case "steam":
                    return player.UserId.Contains(PlayerId) || player.RawUserId.Contains(PlayerId);
                case "discord":
                    return player.UserId.Contains(PlayerId) || player.RawUserId.Contains(PlayerId);
                default:
                    return player.UserId.Contains(PlayerId) || player.RawUserId.Contains(PlayerId);
            }
        }

        public override string ToString()
        {
            return $"{PlayerId}@{Platform}:{Color}:{Text}";
        }
    }
}
