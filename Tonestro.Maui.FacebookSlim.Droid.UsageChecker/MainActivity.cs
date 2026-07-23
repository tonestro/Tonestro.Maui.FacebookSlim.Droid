using System.Text;
using Android.Widget;
using Com.Tonestro.Facebookslim;

namespace Tonestro.Maui.FacebookSlim.Droid.UsageChecker;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        var status = new StringBuilder();
        status.AppendLine("FacebookSlim binding runtime check");
        status.AppendLine();

        AppendCheck(status, "FacebookSdkSlim.SdkVersion", () => FacebookSdkSlim.SdkVersion);

        AppendCheck(status, "Toggle debug via FacebookSdkSlim", () =>
        {
            FacebookSdkSlim.DebugEnabled = true;
            FacebookSdkSlim.AddLoggingBehavior(LoggingBehaviors.AppEvents);
            return $"DebugEnabled={FacebookSdkSlim.DebugEnabled}";
        });

        AppendCheck(status, "AppEventsConstants from bound jar", () => AppEventsConstants.EventNameActivatedApp);

        AppendCheck(status, "AppEventsLoggerSlim.NewLogger + LogEvent", () =>
        {
            var logger = AppEventsLoggerSlim.NewLogger(this) ??
                         throw new NullReferenceException("failed to create facebook app events logger");
            logger.LogEvent("binding_runtime_check", null);
            return logger.Class.Name;
        });

        AppendCheck(status, "LoginManagerSlimFactory.CreateInstance", () =>
        {
            var loginManager = LoginManagerSlimFactory.CreateInstance() ??
                               throw new NullReferenceException("failed to create login manager");
            loginManager.Logout();
            return $"{loginManager.GetType().Name} (Logout() invoked)";
        });

        FindViewById<TextView>(Resource.Id.binding_status)!.Text = status.ToString();
    }

    private static void AppendCheck(StringBuilder status, string description, Func<string?> check)
    {
        try
        {
            status.AppendLine($"[OK] {description}");
            status.AppendLine($"     -> {check()}");
        }
        catch (Exception e)
        {
            status.AppendLine($"[FAILED] {description}");
            status.AppendLine($"     -> {e.Message}");
        }
        status.AppendLine();
    }
}
