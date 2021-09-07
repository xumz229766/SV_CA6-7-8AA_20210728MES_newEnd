using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace desay.Vision
{
    public class Calibration
    {
        //当量 一个像素== mm
        public double dUnit = 0;
        List<MatchResult> m_result = new List<MatchResult>(4);
        MatchResult m_retMove;
        public static double VS_PAI = 3.141596;
        //模板图像
        HTuple m_hPTNMModelID = null;
        public Calibration()
        {
            m_retMove.hv_AngleCheck = 0;
            m_retMove.hv_ColumnCheck = 0;
            m_retMove.hv_RowCheck = 0;
            m_retMove.hv_Score = 0;

            m_result.Add(m_retMove);
            m_result.Add(m_retMove);
            m_result.Add(m_retMove);
            m_result.Add(m_retMove);

        }
        ~Calibration()
        {
            if (m_hPTNMModelID != null)
                HOperatorSet.ClearTemplate(m_hPTNMModelID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_result"></param> List<Match_Result> m_result = new List<Match_Result>(4); 
        /// <param name="dDistance"></param>
        /// <param name="m_dResolutionX"></param>
        /// <param name="m_dResolutionY"></param>
        /// <param name="m_dAngle"></param>
        /// <returns></returns>
        public bool Caculate(double dDistance, out double m_dResolutionX, out double m_dResolutionY, out double m_dAngle)
        {
            m_dResolutionX = 0;
            m_dResolutionY = 0;
            m_dAngle = 0;
            try
            {
                double dX32 = m_result[3].hv_ColumnCheck - m_result[2].hv_ColumnCheck;
                double dY32 = m_result[3].hv_RowCheck - m_result[2].hv_RowCheck;
                double dX10 = m_result[1].hv_ColumnCheck - m_result[0].hv_ColumnCheck;
                double dY10 = m_result[1].hv_RowCheck - m_result[0].hv_RowCheck;

                double dDist1 = Math.Sqrt(dX32 * dX32 + dY32 * dY32);
                double dDist2 = Math.Sqrt(dX10 * dX10 + dY10 * dY10);
                if (dDist1 < 0.00001 && dDist1 > 0.000001) return false;
                if (dDist2 < 0.00001 && dDist2 > 0.000001) return false;
                m_dResolutionX = (dDistance * 2) / dDist1;
                m_dResolutionY = (dDistance * 2) / dDist2;
                m_dAngle = Math.Atan((dY32) / (dX32)) * 180 / VS_PAI;
                return true;
            }
            catch (Exception) { return false; }

        }
        public void Up(HObject m_hImage, HTuple m_hHwd, double dX1, double dX2, double dY1, double dY2)
        {
            CalibPTNM(m_hImage, m_hHwd, 0, dX1, dX2, dY1, dY2);
        }
        public void Down(HObject m_hImage, HTuple m_hHwd, double dX1, double dX2, double dY1, double dY2)
        {
            CalibPTNM(m_hImage, m_hHwd, 1, dX1, dX2, dY1, dY2);
        }
        public void Left(HObject m_hImage, HTuple m_hHwd, double dX1, double dX2, double dY1, double dY2)
        {
            CalibPTNM(m_hImage, m_hHwd, 2, dX1, dX2, dY1, dY2);
        }
        public void Right(HObject m_hImage, HTuple m_hHwd, double dX1, double dX2, double dY1, double dY2)
        {
            CalibPTNM(m_hImage, m_hHwd, 3, dX1, dX2, dY1, dY2);
        }
        private void CalibPTNM(HObject m_hImage, HTuple m_hHwd, int nDir, double dX1, double dX2, double dY1, double dY2)
        {
            try
            {
                MatchResult result;
                BestMatchVis(m_hImage, out result);
                HOperatorSet.SetColor(m_hHwd, "red");
                HOperatorSet.SetDraw(m_hHwd, "margin");
                HOperatorSet.SetLineWidth(m_hHwd, 1);
                HOperatorSet.ClearWindow(m_hHwd);
                HOperatorSet.DispObj(m_hImage, m_hHwd);
                double dd = result.hv_Score[0];
                dd = (255 - dd) / 255 * 100;
                double dX = result.hv_ColumnCheck;
                double dY = result.hv_RowCheck;
                string str;
                if (dd >= 50)
                {

                    double X = dX2 - dX1;
                    double Y = dY2 - dY1;

                    str = ("SUCCESS; X=" + dX.ToString("f2") + ";Y=" + dY.ToString("f2"));
                    HOperatorSet.DispRectangle1(m_hHwd, result.hv_RowCheck - Y / 2, result.hv_ColumnCheck - X / 2, result.hv_RowCheck + Y / 2, result.hv_ColumnCheck + X / 2);

                    m_result[nDir] = result;
                }
                else
                {
                    str = ("Fail");
                }
            }
            catch (Exception ex)
            { }

        }




        /// <summary>
        /// 创建标定模板图像 实现 点动 图像 
        /// </summary>
        /// <param name="hobj"></param>
        /// <param name="dX1"></param>
        /// <param name="dX2"></param>
        /// <param name="dY1"></param>
        /// <param name="dY2"></param>
        /// <returns></returns>
        public bool CreateCalibration(HObject hobj, double dX1, double dX2, double dY1, double dY2)
        {
            if (dX1 == 0 || dX2 == 0 || dY1 == 0 || dY2 == 0)
            {

                dY1 = 960 / 2 - 100;
                dY2 = 960 / 2 + 100;
                dX1 = 1280 / 2 - 100;
                dX2 = 1280 / 2 + 100;

            }
            HObject ho_Rectangle, m_hImageReduced;
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out m_hImageReduced);
            try
            {
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, dY1, dX1, dY2, dX2);
                // 	AreaCenter(ho_Rectangle, &m_VisLib.m_hArea, &m_VisLib.m_hRowRef, &m_VisLib.m_hColumnRef);
                m_hImageReduced.Dispose();
                HOperatorSet.ReduceDomain(hobj, ho_Rectangle, out m_hImageReduced);
                // 	CreateNccModel(m_hImageReduced, "auto", 0, 0, "auto", "use_polarity", &m_VisLib.m_hModelID
                if (m_hPTNMModelID != null)
                    HOperatorSet.ClearTemplate(m_hPTNMModelID);
                HOperatorSet.CreateTemplate(m_hImageReduced, 5, 4, "sort", "original", out m_hPTNMModelID);

                ho_Rectangle.Dispose();
                m_hImageReduced.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                ho_Rectangle.Dispose();
                m_hImageReduced.Dispose();
                return false;
            }
        }
        public bool BestMatchVis(HObject Image, out MatchResult ret)
        {
            ret.hv_AngleCheck = 0;
            ret.hv_ColumnCheck = 0;
            ret.hv_RowCheck = 0;
            ret.hv_Score = 0;
            try
            {
                HOperatorSet.BestMatch(Image, m_hPTNMModelID, 10, "true", out ret.hv_RowCheck, out ret.hv_ColumnCheck, out ret.hv_Score);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
