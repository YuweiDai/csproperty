using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCZJ.Core.Domain.Properties;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using CSCZJ.Services.Property;
using System.Reflection;
using CSCZJ.Core;

namespace CSCZJ.Services.ExportImport
{
    public class ExportManager : IExportManager
    {
        private readonly IPropertyService _propertyService;
        private readonly IMonthTotalService _monthTotalService;
        public ExportManager(IPropertyService propertyService, IMonthTotalService monthTotalService)
        {
            _propertyService = propertyService;
            _monthTotalService = monthTotalService;
        }

        public void ExportPropertyToXlsx(Stream stream, IList<Core.Domain.Properties.Property> properties, IList<string> headers)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("资产导出表");

                for (int i = 0; i < headers.Count(); i++)
                {
                    switch (headers[i])
                    {
                        case "Name":
                            worksheet.Cells[1, i + 1].Value = "资产名称";
                            break;
                        case "Goverment":
                            worksheet.Cells[1, i + 1].Value = "产权单位";
                            break;
                        case "PropertyType":
                            worksheet.Cells[1, i + 1].Value = "资产类别";
                            break;
                        case "Region":
                            worksheet.Cells[1, i + 1].Value = "所处区域";
                            break;
                        case "Address":
                            worksheet.Cells[1, i + 1].Value = "地址";
                            break;
                        case "ConstructArea":
                            worksheet.Cells[1, i + 1].Value = "建筑面积(平方米)";
                            break;
                        case "LandArea":
                            worksheet.Cells[1, i + 1].Value = "土地面积(平方米)";
                            break;
                        case "PropertyID":
                            worksheet.Cells[1, i + 1].Value = "产权证号";
                            break;
                        case "UsedPeople":
                            worksheet.Cells[1, i + 1].Value = "使用方";
                            break;
                        case "ConstructId":
                            worksheet.Cells[1, i + 1].Value = "房产证";
                            break;
                        case "EstateId":
                            worksheet.Cells[1, i + 1].Value = "不动产证";
                            break;
                        case "GetMode":
                            worksheet.Cells[1, i + 1].Value = "获取方式";
                            break;
                        case "FourToStation":
                            worksheet.Cells[1, i + 1].Value = "四至情况";
                            break;
                        case "CurrentType":
                            worksheet.Cells[1, i + 1].Value = "使用现状";
                            break;
                        case "UsedType":
                            worksheet.Cells[1, i + 1].Value = "用途";
                            break;
                    }
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

              

                //Excel添加内容
                int row = 2;
                foreach (var property in properties)
                {
                    Type t = property.GetType();
                    PropertyInfo[] PropertyList = t.GetProperties();
             
                    for (int col = 1; col <= headers.Count(); col++)
                    {
                        foreach (var p in PropertyList)
                        {
                            if (p.Name == headers[col - 1])
                            {
                                if (p.Name == "Goverment")
                                {
                                    var name = property.Government.Name;
                                    worksheet.Cells[row, col].Value = name;
                                }
                                else if (p.Name == "Region") worksheet.Cells[row, col].Value = property.Region.ToDescription();
                                else if (p.Name == "GetMode") worksheet.Cells[row, col].Value = property.GetMode.ToDescription();
                                else if (p.Name == "PropertyType") worksheet.Cells[row, col].Value = property.PropertyType.ToDescription();
                                else if (p.Name == "CurrentType") worksheet.Cells[row, col].Value = property.CurrentType.ToDescription();
                                else worksheet.Cells[row, col].Value = p.GetValue(property);

                            }

                        }
                    }

                    row++;
                }

                xlPackage.Save();

            }



        }



        public void ExportRentsToExl(Stream stream, IList<PropertyRent> rents) {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream)) {
                var worksheet1 = xlPackage.Workbook.Worksheets.Add("出租表");
                var headers1 = new string[] { "资产名称", "出租方", "出租日期", "归还日期", "出租面积(平方米)", "出租总金额(元)", "备注" };
                for (int j = 0; j < headers1.Count(); j++)
                {
                    worksheet1.Cells[1, j + 1].Value = headers1[j];
                    worksheet1.Cells[1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet1.Cells[1, j + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet1.Cells[1, j + 1].Style.Font.Bold = true;
                }

                int row1 = 2;
                foreach (var rent in rents)
                {
                    int col1 = 1;
                    worksheet1.Cells[row1, col1].Value = rent.Title;
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.Name;
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.RentTime.ToString("yyyy-MM-dd");
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.BackTime.ToString("yyyy-MM-dd");
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.RentArea;
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.PriceString;
                    col1++;

                    worksheet1.Cells[row1, col1].Value = rent.Remark;
                    col1++;

                    row1++;
                }

                xlPackage.Save();
            }
          


        }


        public void ExportMonthTotal(Stream stream, List<Core.Domain.Properties.Property> properties, int id,string month)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("全部记录表");

                var headers = new string[] { "资产名称", "资产类别", "产权单位", "资产所在地", "建筑面积", "对应土地面积", "账面价值", "土地面积", "账面价值", "自用", "出租", "出借", "闲置", "本月租金收益" };
                for (int j = 0; j < headers.Count(); j++)
                {
                    worksheet.Cells[1, j + 1].Value = headers[j];
                    worksheet.Cells[1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, j + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, j + 1].Style.Font.Bold = true;
                }

                var worksheet1 = xlPackage.Workbook.Worksheets.Add("统计表");
                var headers1 = new string[] { "单位性质", "建筑面积", "对应土地面积", "账面价值", "土地面积", "账面价值", "自用", "出租", "出借", "闲置", "本月租金收益" };
                for (int k = 0; k < headers1.Count(); k++)
                {
                    worksheet1.Cells[1, k + 1].Value = headers1[k];
                    worksheet1.Cells[1, k + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet1.Cells[1, k + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet1.Cells[1, k + 1].Style.Font.Bold = true;
                }

                int row = 2;
                var xProperties = new List<Core.Domain.Properties.Property>();
                var sProperties = new List<Core.Domain.Properties.Property>();
                var qProperties = new List<Core.Domain.Properties.Property>();

                foreach (var property in properties)
                {
                    if (property.Government.GovernmentType == GovernmentType.Government)
                    {
                        xProperties.Add(property);
                    }
                    else if (property.Government.GovernmentType == GovernmentType.Institution)
                    {
                        sProperties.Add(property);
                    }
                    else
                    {
                        qProperties.Add(property);
                    }

                    worksheet.Cells[row, 1].Value = property.Name;
                    worksheet.Cells[row, 2].Value = property.PropertyType.ToDescription();
                    worksheet.Cells[row, 3].Value = property.Government.Name;
                    worksheet.Cells[row, 4].Value = property.Address;
                    worksheet.Cells[row, 5].Value = property.ConstructArea;
                    worksheet.Cells[row, 6].Value = property.ConstructArea;
           
                    worksheet.Cells[row, 8].Value = property.LandArea;
          

                    var monthtotals = _monthTotalService.GetPropertyMonthTotal(property.Id, month).ToList();

                    if (monthtotals.Count() > 0)
                    {
                        var monthtotal = monthtotals[monthtotals.Count() - 1];
                        worksheet.Cells[row, 10].Value = monthtotal.CurrentUse_Self;
                        worksheet.Cells[row, 11].Value = monthtotal.CurrentUse_Rent;
                        worksheet.Cells[row, 12].Value = monthtotal.CurrentUse_Lend;
                        worksheet.Cells[row, 13].Value = monthtotal.CurrentUse_Idle;
                        worksheet.Cells[row, 14].Value = monthtotal.Income;
                    }
                    else
                    {
                 
                        worksheet.Cells[row, 14].Value = 0;
                    }

                    row++;
                }
                double sum = 0; double sum1 = 0; double sum2 = 0; double sum3 = 0; double sum4 = 0; double sum5 = 0; double sum6 = 0; double sum7 = 0; double sum8 = 0; double sum9 = 0;
                double asum = 0; double asum1 = 0; double asum2 = 0; double asum3 = 0; double asum4 = 0; double asum5 = 0; double asum6 = 0; double asum7 = 0; double asum8 = 0; double asum9 = 0;


                #region 财政局导出的数据
                if (id == 37)
                {

                    for (int i = 0; i < xProperties.Count(); i++)
                    {
                        var monthtotals = _monthTotalService.GetPropertyMonthTotal(xProperties[i].Id, month).ToList();
                        if (monthtotals.Count() > 0)
                        {
                            var monthtotal = monthtotals[monthtotals.Count() - 1];
                            sum += xProperties[i].ConstructArea;
                            sum1 += xProperties[i].ConstructArea;
                          
                            sum3 += xProperties[i].LandArea;
                            
                            sum5 += monthtotal.CurrentUse_Self;
                            sum6 += monthtotal.CurrentUse_Rent;
                            sum7 += monthtotal.CurrentUse_Lend;
                            sum8 += monthtotal.CurrentUse_Idle;
                            sum9 += monthtotal.Income;
                        }
                        else
                        {
                            sum += xProperties[i].ConstructArea;
                            sum1 += xProperties[i].ConstructArea;
                            sum3 += xProperties[i].LandArea;

                            sum9 += 0;
                        }
                    }
                    for (int i = 0; i < sProperties.Count(); i++)
                    {
                        var monthtotals = _monthTotalService.GetPropertyMonthTotal(sProperties[i].Id, month).ToList();
                        if (monthtotals.Count() > 0)
                        {
                            var monthtotal = monthtotals[monthtotals.Count() - 1];
                            asum += sProperties[i].ConstructArea;
                            asum1 += sProperties[i].ConstructArea;
               
                            asum3 += sProperties[i].LandArea;
                  
                            asum5 += monthtotal.CurrentUse_Self;
                            asum6 += monthtotal.CurrentUse_Rent;
                            asum7 += monthtotal.CurrentUse_Lend;
                            asum8 += monthtotal.CurrentUse_Idle;
                            asum9 += monthtotal.Income;
                        }
                        else
                        {
                            asum += sProperties[i].ConstructArea;
                            asum1 += sProperties[i].ConstructArea;
                            asum3 += sProperties[i].LandArea;                
                            asum9 += 0;
                        }
                    }


                    worksheet1.Cells[2, 1].Value = "行政单位";
                    worksheet1.Cells[2, 2].Value = sum;
                    worksheet1.Cells[2, 3].Value = sum1;
                    worksheet1.Cells[2, 4].Value = sum2;
                    worksheet1.Cells[2, 5].Value = sum3;
                    worksheet1.Cells[2, 6].Value = sum4;
                    worksheet1.Cells[2, 7].Value = sum5;
                    worksheet1.Cells[2, 8].Value = sum6;
                    worksheet1.Cells[2, 9].Value = sum7;
                    worksheet1.Cells[2, 10].Value = sum8;
                    worksheet1.Cells[2, 11].Value = sum9;

                    worksheet1.Cells[3, 1].Value = "事业单位";
                    worksheet1.Cells[3, 2].Value = asum;
                    worksheet1.Cells[3, 3].Value = asum1;
                    worksheet1.Cells[3, 4].Value = asum2;
                    worksheet1.Cells[3, 5].Value = asum3;
                    worksheet1.Cells[3, 6].Value = asum4;
                    worksheet1.Cells[3, 7].Value = asum5;
                    worksheet1.Cells[3, 8].Value = asum6;
                    worksheet1.Cells[3, 9].Value = asum7;
                    worksheet1.Cells[3, 10].Value = asum8;
                    worksheet1.Cells[3, 11].Value = asum9;

                }

                #endregion


                #region 国资委导出的数据
                else if (id == 26)
                {
                    for (int i = 0; i < qProperties.Count(); i++)
                    {
                        var monthtotals = _monthTotalService.GetPropertyMonthTotal(qProperties[i].Id, month).ToList();
                        if (monthtotals.Count() > 0)
                        {
                            var monthtotal = monthtotals[monthtotals.Count() - 1];
                            sum += qProperties[i].ConstructArea;
                            sum1 += qProperties[i].ConstructArea;
                            sum3 += qProperties[i].LandArea;
                            sum5 += monthtotal.CurrentUse_Self;
                            sum6 += monthtotal.CurrentUse_Rent;
                            sum7 += monthtotal.CurrentUse_Lend;
                            sum8 += monthtotal.CurrentUse_Idle;
                            sum9 += monthtotal.Income;
                        }
                        else
                        {
                            sum += qProperties[i].ConstructArea;
                            sum1 += qProperties[i].ConstructArea;
                            sum3 += qProperties[i].LandArea;
                            sum9 += 0;
                        }

                        worksheet1.Cells[2, 1].Value = "企业单位";
                        worksheet1.Cells[2, 2].Value = sum;
                        worksheet1.Cells[2, 3].Value = sum1;
                        worksheet1.Cells[2, 4].Value = sum2;
                        worksheet1.Cells[2, 5].Value = sum3;
                        worksheet1.Cells[2, 6].Value = sum4;
                        worksheet1.Cells[2, 7].Value = sum5;
                        worksheet1.Cells[2, 8].Value = sum6;
                        worksheet1.Cells[2, 9].Value = sum7;
                        worksheet1.Cells[2, 10].Value = sum8;
                        worksheet1.Cells[2, 11].Value = sum9;

                    }
                }
                #endregion


                xlPackage.Save();

                

            }


        }

    }
}