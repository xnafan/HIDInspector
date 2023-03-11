using HIDInterface;
using System.Text;
using static HIDInterface.HIDDevice;

namespace HIDInspector;
public partial class Form1 : Form
{
    private HIDDevice? _device;
    private StringBuilder _builder = new();
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        lstHidDevices.DataSource = HIDDevice.getConnectedDevices();
    }

    private void lstHidDevices_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstHidDevices.SelectedItem == null) { return; }
        propertyGrid1.SelectedObject = lstHidDevices.SelectedItem;
        if (_device != null) {  _device.dataReceived -= data => AddDataToTextOutput(data); _device.close(); _device = null; }
        _device = new HIDDevice(((InterfaceDetails)lstHidDevices.SelectedItem).DevicePath, true);

        //subscribe to data received event
        _device.dataReceived += data => AddDataToTextOutput(data);
    }


    public void AddDataToTextOutput(byte[] data)
    {
        try
        {
            if (InvokeRequired) { Invoke(() => AddDataToTextOutput(data)); }
            else {
                foreach (var dataByte in data)
                {
                    _builder.Append(Convert.ToString(dataByte, 2).PadLeft(8, '0') + " ");
                }
                txtOutput.AppendText(_builder.ToString() + Environment.NewLine);
                _builder.Clear();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}