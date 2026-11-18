using System;
using System.Drawing;
using System.Windows.Forms;

namespace BatteryIcon
{
    // Live battery details. The timer only exists while the window is open,
    // so nothing is read in the background when it's closed.
    // Closes like a flyout: Esc (CancelButton) or clicking anywhere else.
    public partial class DetailsForm : Form
    {
        private Timer refreshTimer;

        public DetailsForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // the model doesn't change, so it's read once
            modelValue.Text = BatteryInfo.GetModel() ?? "n/a";

            refreshTimer = new Timer();
            refreshTimer.Interval = CustomSettings.Instance.updateInterval;
            refreshTimer.Tick += (sender, e) => RefreshValues();
        }

        protected override void OnLoad(EventArgs e)
        {
            // the form is scaled to the screen's DPI in base.OnLoad,
            // so the first refresh (which sizes the window) comes after it
            base.OnLoad(e);
            RefreshValues();
            refreshTimer.Start();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // a window opened from the tray menu doesn't always get focus by itself;
            // without focus neither Esc nor clicking elsewhere would reach it
            Activate();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            // closing with Esc also deactivates the window; don't close twice
            if (!closing)
            {
                Close();
            }
        }

        private bool closing;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            closing = true;
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            base.OnFormClosed(e);
        }

        private void RefreshValues()
        {
            BatteryInfo.State state;
            if (BatteryInfo.TryGetState(out state))
            {
                powerLineValue.Text = state.PowerOnline ? "Connected" : "Disconnected";
                if (state.RateMw < 0)
                {
                    rateLabel.Text = "Discharge rate";
                    rateValue.Text = String.Format("{0:F1} W", -state.RateMw.Value / 1000.0);
                }
                else
                {
                    rateLabel.Text = "Charge rate";
                    rateValue.Text = state.RateMw.HasValue ? String.Format("{0:F1} W", state.RateMw.Value / 1000.0) : "n/a";
                }
            }
            else
            {
                powerLineValue.Text = "n/a";
                rateValue.Text = "n/a";
            }

            BatteryInfo.Capacity capacity;
            if (BatteryInfo.TryGetCapacity(out capacity))
            {
                designCapacityValue.Text = String.Format("{0:N0} mWh", capacity.DesignMwh);
                fullChargeCapacityValue.Text = String.Format("{0:N0} mWh", capacity.FullChargeMwh);
                healthValue.Text = String.Format("{0:F0}%", 100.0 * capacity.FullChargeMwh / capacity.DesignMwh);
            }
            else
            {
                designCapacityValue.Text = "n/a";
                fullChargeCapacityValue.Text = "n/a";
                healthValue.Text = "n/a";
            }

            FitWidth();
        }

        // Widen the window when a value (usually the model name) doesn't fit.
        // Only grows, so the window doesn't jump around while values change.
        private void FitWidth()
        {
            int needed = 0;
            foreach (Label value in new[] { rateValue, powerLineValue, designCapacityValue, fullChargeCapacityValue, healthValue, modelValue })
            {
                needed = Math.Max(needed, value.Left + value.PreferredWidth);
            }
            // same margin on the right as on the left
            needed += rateLabel.Left;
            if (needed > ClientSize.Width)
            {
                // grow equally to both sides so the window stays centered;
                // the Close button is anchored to the right, so it moves along
                int growth = needed - ClientSize.Width;
                ClientSize = new Size(needed, ClientSize.Height);
                Left -= growth / 2;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
