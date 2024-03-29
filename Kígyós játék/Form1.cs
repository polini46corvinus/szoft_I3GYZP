using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Kígyós_játék
{
    public partial class Form1 : Form
    {
        public Form1() { InitializeComponent(); }
        private void Form1_Load(object sender, EventArgs e) { }

        int fej_x = 200;
        int fej_y = 200;

        int irány_x = 1;
        int irány_y = 0;

        int lépésszám = 0;
        int hossz = 5;
        int szint = 1;

        bool gombnyomas = false;
        bool paused = false;

        List<KígyóElem> kígyólista = new List<KígyóElem>();
        List<Alma> almalista = new List<Alma>();
        List<Méreg> mereglista = new List<Méreg>();

        public void Halal()
        {
            timer1.Enabled = false;
            kajagenerátorTimer.Enabled = false;
            MessageBox.Show("GAME OVER");
            Close();
            return;
        }
        
        public void Pause()
        {
            paused = true;
            timer1.Enabled = false;
            kajagenerátorTimer.Enabled = false;
            Label pausefelirat = new Label();
            pausefelirat.Top = ClientRectangle.Height / 2-10;
            pausefelirat.Left = ClientRectangle.Width / 2-30;
            pausefelirat.Text = "PAUSED";
            Controls.Add(pausefelirat);
        }

        public void continuePause()
        {
            timer1.Enabled = true;
            kajagenerátorTimer.Enabled = true;
            paused = false;
            Controls.RemoveAt(Controls.Count-1);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lépésszám++;

            fej_x += irány_x * KígyóElem.Méret;
            fej_y += irány_y * KígyóElem.Méret;

            //saját farok halál
            foreach (KígyóElem item in kígyólista) { if (item.Top == fej_y && item.Left == fej_x) { Halal(); } }

            //egyél ALMÁT
            foreach (Alma alma1 in almalista)
            {
                if (alma1.Top == fej_y && alma1.Left == fej_x)
                {
                    hossz++;
                    almalista.Remove(alma1);
                    Controls.Remove(alma1);

                    if (hossz % 10 == 0)
                    {
                        szint = (hossz / 10) + 1;
                        timer1.Interval = Convert.ToInt16(timer1.Interval * 0.9);
                    }
                    break;
                }
            }

            //egyél MÉRGET
            foreach (Méreg mereg1 in mereglista)
            {
                if (mereg1.Top == fej_y && mereg1.Left == fej_x) {Halal(); break; }
            }

            //fejnövesztés
            KígyóElem ke = new KígyóElem();
            ke.Top = fej_y;
            ke.Left = fej_x;
            if (lépésszám % 2 == 0) ke.BackColor = Color.Black; //nekem így jobban tetszik na :(
            Controls.Add(ke);
            kígyólista.Add(ke);
            lepesszamLabel.Text = "Lépésszám: " + lépésszám.ToString();
            hosszLabel.Text = "Hossz: " + hossz.ToString();
            szintLabel.Text = "Szint: " + szint.ToString();
            gombnyomas = false;

            if (kígyólista.Count > hossz)
            {
                KígyóElem levágandó = kígyólista[0];
                kígyólista.RemoveAt(0);
                Controls.Remove(levágandó);
            }

            //out of bounds halál
            if (fej_x < 0 ||
                fej_y < 0 ||
                fej_x > ClientRectangle.Width - KígyóElem.Méret ||
                fej_y > ClientRectangle.Height - KígyóElem.Méret) { Halal(); }
        }

        private void lefele(object sender, KeyEventArgs e)
        {
            if (paused == true) {if (e.KeyCode == Keys.Escape) { continuePause(); }}

            if (gombnyomas == false && paused == false) //pufferszűrő
            {
                if (e.KeyCode == Keys.Up && irány_y != 1) { irány_y = -1; irány_x = 0; }
                if (e.KeyCode == Keys.Down && irány_y != -1) { irány_y = 1; irány_x = 0; }
                if (e.KeyCode == Keys.Left && irány_x != 1) { irány_y = 0; irány_x = -1; }
                if (e.KeyCode == Keys.Right && irány_x != -1) { irány_y = 0; irány_x = 1; }
                if (e.KeyCode == Keys.Escape) { Pause(); }
                gombnyomas = true;
            }
        }
        private void kajagenerátorTimer_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int yloc = rnd.Next(ClientRectangle.Height / KígyóElem.Méret)*20;
            int xloc = rnd.Next(ClientRectangle.Width / KígyóElem.Méret)*20;
            int esély = rnd.Next(10);

            bool vanEOttValami = false;

            foreach (Control control in Controls)
            {
                if (control.Top != yloc && control.Left != xloc) { vanEOttValami = false; }
                else { vanEOttValami = true;}
            }
            if (vanEOttValami == false)
            {
                if (esély > 2) //csinálj almát
                {
                    Alma ujalma = new Alma();
                    ujalma.Top = yloc;
                    ujalma.Left = xloc;
                    Controls.Add(ujalma);
                    almalista.Add(ujalma);
                }
                else  //csinálj mérget
                {
                    Méreg ujmereg = new Méreg();
                    ujmereg.Top = yloc;
                    ujmereg.Left = xloc;
                    if (mereglista.Count < 3)
                    {
                        Controls.Add(ujmereg);
                        mereglista.Add(ujmereg);
                    }
                    else
                    {
                        KígyóElem levágandó = kígyólista[0];
                        kígyólista.RemoveAt(0);
                        Controls.Remove(levágandó);

                        Méreg törlendőMéreg = mereglista[0];
                        mereglista.RemoveAt(0);
                        Controls.Remove(törlendőMéreg);
                    }
                }
            }
        }
    }
}