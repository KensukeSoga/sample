using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Isid.KKH.Common.KKHView.Common.Form;
using Isid.KKH.Common.KKHProcessController.MasterMaintenance;
using Isid.KKH.Common.KKHView.Mast;
using Isid.NJ.View.Navigator;
using Isid.KKH.Common.KKHSchema;
using Isid.KKH.Common.KKHView.Common;
using Isid.KKH.Common.KKHUtility.Constants;
using FarPoint.Win.Spread.Model;

namespace Isid.KKH.Skyp.View.Mast
{
    public partial class frmMastSkyp : MastForm, INaviParameter
    {
        #region メンバ変数

        /// <summary>
        /// ナビパラメーター
        /// </summary>
        KKHNaviParameter mstNavPrm = new KKHNaviParameter();

        /// <summary>
        /// dataModelChangeイベント 
        /// </summary>
        DefaultSheetDataModel dataModel;

        # endregion

        #region コンストラクタ

        public frmMastSkyp()
        {
            InitializeComponent();
        }

        # endregion

        # region イベント

        /// <summary>
        /// フォーム画面表示処理 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void frmMastSkyp_ProcessAffterNavigating(object sender, Isid.NJ.View.Base.NavigationEventArgs arg)
        {
            mstNavPrm = (KKHNaviParameter)arg.NaviParameter;
        }

        /// <summary>
        /// タイムスポット区分チェック  
        /// </summary>
        /// <returns></returns>
        protected override bool MstChk()
        {
            if (!base.MstChk()) { return false; }

            // 分類マスターの時のみチェックを行う
            if (mstDtSet.MasterDataSet.MasterKindVO[cmbMasterName1.SelectedIndex].field3.Trim() == "0001")
            {
                //[業務区分]列Index
                int gyomColIdx = -1;
                //[タイムスポット]列Index
                int tsColIdx = -1;

                //[業務区分][タイムスポット区分]列のIndexを取得する 
                for (int i = 0; i < spdMasMainte_Sheet1.Columns.Count; i++)
                {
                    if (spdMasMainte_Sheet1.ColumnHeader.Columns[i].Label =="業務区分")
                    {
                        gyomColIdx = i;
                    }
                    else if (spdMasMainte_Sheet1.ColumnHeader.Columns[i].Label == "タイムスポット区分")
                    {
                        tsColIdx = i;
                    }
                }

                for (int i = 0; i < spdMasMainte_Sheet1.Rows.Count; i++)
                {

                    //ラジオ・衛星メディアの場合はエラーメッセージ（更新不可） 
                    if (spdMasMainte_Sheet1.Cells[i, gyomColIdx].Text.ToString() == "ラジオ"
                        || spdMasMainte_Sheet1.Cells[i, gyomColIdx].Text.ToString() == "衛星メディア")
                    {
                            //タイムスポット区分が空の場合 
                        if (spdMasMainte_Sheet1.Cells[i, tsColIdx].Text.ToString() == "")
                        {
                            MessageUtility.ShowMessageBox(MessageCode.HB_W0136, null,
                                "マスタメンテナンス", MessageBoxButtons.OK);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMastSkyp_Load(object sender, EventArgs e)
        {
            dataModel = (DefaultSheetDataModel)spdMasMainte.ActiveSheet.Models.Data;
            dataModel.Changed += new SheetDataModelEventHandler(this.dataModel_Changed);
        }

        /// <summary>
        /// 更新ボタン押下 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void btnUpd_Click(object sender, EventArgs e)
        {
            dataModel.Changed -= new SheetDataModelEventHandler(this.dataModel_Changed);
            base.btnUpd_Click(sender, e);
            dataModel.Changed += new SheetDataModelEventHandler(this.dataModel_Changed);
        }

        # endregion イベント 
 
        # region メソッド

        /// <summary>
        /// dataModel_Changed(ペースト対策) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataModel_Changed(object sender, SheetDataModelEventArgs e)
        {
            int gyomColIdx = -1; //[業務区分]列Index
            int tsColIdx = -1;   //[タイムスポット]列Index

            //[業務区分][国際区分][タイムスポット区分][コード]列のIndexを取得する 
            for (int i = 0; i < spdMasMainte_Sheet1.Columns.Count; i++)
            {
                if (spdMasMainte_Sheet1.ColumnHeader.Columns[i].Label == "業務区分")
                {
                    gyomColIdx = i;
                }
                else if (spdMasMainte_Sheet1.ColumnHeader.Columns[i].Label == "タイムスポット区分")
                {
                    tsColIdx = i;
                }
            }
            //[業務区分]列の場合、処理する 
            if (e.Column == gyomColIdx && gyomColIdx > -1)
            {
                //業務区分がラジオ、または衛星メディアの場合 
                if (spdMasMainte_Sheet1.Cells[e.Row, gyomColIdx].Text.ToString() == "ラジオ"
                    || spdMasMainte_Sheet1.Cells[e.Row, gyomColIdx].Text.ToString() == "衛星メディア")
                {
                    if (tsColIdx > -1)
                    {
                        //タイムスポット区分初期化 
                        //編集可  
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].Locked = false;
                        //背景色を白 
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].BackColor = Color.White;
                        //フォーカス可 
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].CanFocus = true;
                        //値を空にする  
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].Value = "";
                    }
                }
                else
                {
                    if (tsColIdx > -1)
                    {
                        //タイムスポット区分初期化 
                        //編集可  
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].Locked = true;
                        //背景色を白 
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].BackColor = Color.Black;
                        //フォーカス可 
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].CanFocus = false;
                        //値を空にする  
                        spdMasMainte_Sheet1.Cells[e.Row, tsColIdx].Value = "";
                    }
                }
            }
        }

        # endregion メソッド 



    }
}

