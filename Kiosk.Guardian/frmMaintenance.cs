﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public partial class frmMaintenance : Form
    {
        public frmMaintenance()
        {
            InitializeComponent();
            this.KeyPress += FrmMaintenance_KeyPress;
        }

        private void FrmMaintenance_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void frmMaintenance_Load(object sender, EventArgs e)
        {
            lblErrorMessages.Visible = false;
        }

        private void btnTurnOff_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void SetNumber(string number)
        {
            lblErrorMessages.Visible = false;
            txtPassword.Text += number;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetNumber("1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetNumber("2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetNumber("3");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetNumber("4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetNumber("5");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetNumber("6");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetNumber("7");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetNumber("8");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetNumber("9");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetNumber("0");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPassword.Text = "";
            lblErrorMessages.Visible = false;
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            if(txtPassword.Text == "0789")
            {
                this.Close();
            }
            else
            {
                lblErrorMessages.Text = "SENHA INVÁLIDA";
                lblErrorMessages.Visible = true;
            }
        }
    }
}
