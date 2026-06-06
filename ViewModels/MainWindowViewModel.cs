namespace PasswordResetSimulator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string OriginalPassword { get; set; }
        = "Password not generated";

    public string FoundPassword { get; set; }
        = "Not found";

    public string ElapsedTime { get; set; }
        = "0 sec";

    public string SingleThreadTime { get; set; }
        = "-";

    public string MultiThreadTime { get; set; }
        = "-";

    public string Speedup { get; set; }
        = "-";
}