using WindowsInput;
using WindowsInput.Native;

public class WhatsAppAutomation
{
    private static readonly InputSimulator sim = new InputSimulator();

    public static void SendMessageToWhatsApp(string message)
    {
        // Вставить текст как Ctrl+V (предполагается, что текст уже в буфере обмена)
        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        Thread.Sleep(1000);

        // Enter для отправки
        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
    }

    public static void SendKeysOneByOne(string text)
    {
        foreach (char c in text)
        {
            sim.Keyboard.TextEntry(c);
            Thread.Sleep(50); // можно убрать, если не нужно замедление
        }

        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN); // Enter в конце
    }
    public static void Enter()
    {
        Thread.Sleep(3000);
        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
    }
    public static void CTRLV()
    {
        Thread.Sleep(3000);
        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V); //
    }
    public static void BOTTOM()
    {
        Thread.Sleep(3000);
        sim.Keyboard.KeyPress(VirtualKeyCode.DOWN);
    }
    public static void AltTabBack()
    {
        Thread.Sleep(3000);
        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.TAB); // ALT+TAB
    }

    public static void CloseTab()
    {
        Thread.Sleep(2000);
        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.F4); // CTRL+F4
    }
    public static void Left()
    {
        Thread.Sleep(2000);
        sim.Keyboard.KeyPress(VirtualKeyCode.LEFT); // CTRL+F4
    }
}