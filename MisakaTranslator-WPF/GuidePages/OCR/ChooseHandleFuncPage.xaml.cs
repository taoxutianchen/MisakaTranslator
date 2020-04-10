﻿using OCRLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MisakaTranslator_WPF.GuidePages.OCR
{
    /// <summary>
    /// ChooseHandleFuncPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseHandleFuncPage : Page
    {
        public List<string> ImageProcFunclist;
        public List<string> Langlist;

        public ChooseHandleFuncPage()
        {
            InitializeComponent();

            ImageProcFunclist = ImageProcFunc.lstHandleFun.Keys.ToList();
            HandleFuncCombox.ItemsSource = ImageProcFunclist;
            HandleFuncCombox.SelectedIndex = 0;

            Langlist = ImageProcFunc.lstOCRLang.Keys.ToList();
            OCRLangCombox.ItemsSource = Langlist;
            OCRLangCombox.SelectedIndex = 1;

        }

        private void RenewAreaBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Image img = Common.ocr.GetOCRAreaCap();
            
            SrcImg.Source = ImageProcFunc.ImageToBitmapImage(img);

            DstImg.Source = ImageProcFunc.ImageToBitmapImage(ImageProcFunc.Auto_Thresholding(new System.Drawing.Bitmap(img), 
                ImageProcFunc.lstHandleFun[ImageProcFunclist[HandleFuncCombox.SelectedIndex]]));

            GC.Collect();
        }

        private void OCRTestBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (Common.appSettings.OCRsource == "TesseractOCR")
            {
                if (Common.ocr.OCR_Init("", "") != false)
                {
                    string res = Common.ocr.OCRProcess();

                    if (res != null)
                    {
                        HandyControl.Controls.MessageBox.Show(res, "识别结果");
                    }
                    else {
                        HandyControl.Controls.Growl.Error("TesseractOCR 工作异常\n" + Common.ocr.GetLastError());
                    }
                }
                else
                {
                    HandyControl.Controls.Growl.Error("TesseractOCR 工作异常\n" + Common.ocr.GetLastError());
                }
            }
            else if (Common.appSettings.OCRsource == "BaiduOCR")
            {
                if (Common.ocr.OCR_Init(Common.appSettings.BDOCR_APIKEY, Common.appSettings.BDOCR_SecretKey) != false)
                {
                    string res = Common.ocr.OCRProcess();

                    if (res != null)
                    {
                        HandyControl.Controls.MessageBox.Show(res, "识别结果");
                    }
                    else
                    {
                        HandyControl.Controls.Growl.Error("百度OCR API工作异常\n" + Common.ocr.GetLastError());
                    }
                }
                else {
                    HandyControl.Controls.Growl.Error("百度OCR API工作异常\n" + Common.ocr.GetLastError());
                }
            }
            
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            Common.GameID = -1;//OCR暂时还不支持保存,因此强制给值-1

            //使用路由事件机制通知窗口来完成下一步操作
            PageChangeRoutedEventArgs args = new PageChangeRoutedEventArgs(PageChange.PageChangeRoutedEvent, this);
            args.XamlPath = "GuidePages/OCR/ChooseHotKeyPage.xaml";
            this.RaiseEvent(args);
        }

        private void OCRLangCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Common.ocr.SetOCRSourceLang(ImageProcFunc.lstOCRLang[Langlist[OCRLangCombox.SelectedIndex]]);
        }

        private void HandleFuncCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Drawing.Image img = Common.ocr.GetOCRAreaCap();

            SrcImg.Source = ImageProcFunc.ImageToBitmapImage(img);

            DstImg.Source = ImageProcFunc.ImageToBitmapImage(ImageProcFunc.Auto_Thresholding(new System.Drawing.Bitmap(img),
                ImageProcFunc.lstHandleFun[ImageProcFunclist[HandleFuncCombox.SelectedIndex]]));

            GC.Collect();
        }
    }
}