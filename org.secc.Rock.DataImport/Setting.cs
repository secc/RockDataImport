using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace org.secc.Rock.DataImport
{
    public class Setting
    {
        #region Properties
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        #endregion

        #region Constructor
        public Setting() { }

        public Setting( string category, string name, string value )
        {
            Category = category;
            Name = name;
            Value = value;
        }
        #endregion

        #region Public

        public static string GetApplicationSetting( string settingKey )
        {
            var asr = new System.Configuration.AppSettingsReader();

            return (string)asr.GetValue( settingKey, typeof( string ) );
        }

        public static string GetPluginFolderPath()
        {
            return System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Plugins" );
        }

        public static List<Setting> GetSettingsByCategory( string categoryName )
        {

            return App.ApplicaitonSettings.Where( a => a.Category == categoryName ).ToList();
        }


        public static int GetImportBatchSize()
        {
            string batchSizeString = GetApplicationSetting( "BatchSize" );
            int batchSize = 0;

            int.TryParse( batchSizeString, out batchSize );

            return batchSize;
        }

        public static int GetMaxFailureCount()
        {
            string maxFailureSettingValue = GetApplicationSetting( "MaximumFailureCount" );
            int maxFailures = 0;

            int.TryParse( maxFailureSettingValue, out maxFailures );

            return maxFailures;
        }

        public static int GetMaxRecordsToImport()
        {
            string maxRecordsSettingValue = GetApplicationSetting( "RecordsToImport" );
            int maxRecords = 0;

            int.TryParse( maxRecordsSettingValue, out maxRecords );

            return maxRecords;
        }

        public static string GetSettingValue( string name )
        {
            return GetSettingValue( String.Empty, name );
        }

        public static string GetSettingValue( string category, string name )
        {
            return App.ApplicaitonSettings
                    .Where( s => s.Category == category )
                    .Where( s => s.Name == name )
                    .Select( s => s.Value )
                    .FirstOrDefault();
        }

        public static void LoadSettings()
        {
            if ( App.ApplicaitonSettings == null )
            {
                App.ApplicaitonSettings = new List<Setting>();
            }
            else
            {
                App.ApplicaitonSettings.Clear();
            }

            string settingPath = GetSettingsPath();

            if ( File.Exists( settingPath ) )
            {
                XmlSerializer reader = new XmlSerializer( typeof( List<Setting> ) );

                using ( StreamReader sr = new StreamReader( settingPath ) )
                {
                    App.ApplicaitonSettings = (List<Setting>)reader.Deserialize( sr );
                }
            }
        }

        public static void SaveSettings()
        {
            if ( App.ApplicaitonSettings == null )
            {
                return;
            }

            string settingsPath = GetSettingsPath();

            XmlSerializer serial = new XmlSerializer( typeof( List<Setting> ) );

            using ( StreamWriter sw = new StreamWriter( settingsPath, false ) )
            {
                serial.Serialize( sw, App.ApplicaitonSettings );
            }
        }

        public static void UpdateSettingValue(string category, string name, string value, bool saveSettingFile = false)
        {
            var setting = App.ApplicaitonSettings
                .Where( s => s.Category == category )
                .Where( s => s.Name == name )
                .FirstOrDefault();


            if ( setting == null )
            {
                if ( !String.IsNullOrWhiteSpace( value ) )
                {
                    App.ApplicaitonSettings.Add( new Setting( category, name, value ) );
                }

            }
            else
            {
                if ( !String.IsNullOrWhiteSpace( value ) )
                {
                    setting.Value = value;
                }
                else
                {
                    App.ApplicaitonSettings.Remove( setting );
                }
            }

            if ( saveSettingFile )
            {
                SaveSettings();
            }
        }


        #endregion


        #region Private

        private static string GetSettingsPath()
        {
            return string.Format( "{0}Settings.xml", AppDomain.CurrentDomain.BaseDirectory );
        }

        #endregion
    }
}
