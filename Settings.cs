using System;

namespace busylight_server
{
    public class Settings
    {
        public static string API_KEYS { private set; get; }

        public Settings()
        {
            foreach (var prop in this.GetType().GetProperties())
                prop.SetValue(this, Environment.GetEnvironmentVariable(prop.Name));

        }

    }
}
