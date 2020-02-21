﻿using CsvHelper;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using Magicodes.ExporterAndImporter.Tests.Models.Export;
using Shouldly;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Magicodes.ExporterAndImporter.Tests
{
    public class ExcelCsvExporter_Tests : TestBase
    {

        [Fact(DisplayName = "大量数据导出Excel")]
        public async Task Export_Test()
        {
            IExporter exporter = new CsvExporter();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), nameof(Export_Test) + ".csv");
            if (File.Exists(filePath)) File.Delete(filePath);

            var result =
                await exporter.Export(filePath, GenFu.GenFu.ListOf<ExportTestData>(100000));
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }


        [Fact(DisplayName = "Dto导出")]
        public async Task ExportAsByteArray_Test()
        {
            IExporter exporter = new CsvExporter();
            var filePath = GetTestFilePath($"{nameof(ExportAsByteArray_Test)}.csv");
            DeleteFile(filePath);
            var result = await exporter.ExportAsByteArray(GenFu.GenFu.ListOf<ExportTestData>());
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            File.WriteAllBytes(filePath, result);
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "DTO特性导出（测试格式化）")]
        public async Task AttrsExport_Test()
        {
            IExporter exporter = new CsvExporter();

            var filePath = GetTestFilePath($"{nameof(AttrsExport_Test)}.csv");

            DeleteFile(filePath);

            var data = GenFu.GenFu.ListOf<ExportTestDataWithAttrs>(100);
            foreach (var item in data)
            {
                item.LongNo = 45875266524;
            }

            var result = await exporter.Export(filePath, data);

            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<AutoMap<ExportTestDataWithAttrs>>();
                var exportDatas = csv.GetRecords<ExportTestDataWithAttrs>().ToList();
                exportDatas.Count().ShouldBe(100);
                var exportData = exportDatas.FirstOrDefault();
                exportData.Time1.ToString().ShouldBeGreaterThanOrEqualTo(exportData.Time1.ToString("yyyy-MM-dd"));
                exportData.Time2.ToString()
                    .ShouldBeGreaterThanOrEqualTo(exportData.Time2?.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        [Fact(DisplayName = "空数据导出")]
        public async Task AttrsExportWithNoData_Test()
        {
            IExporter exporter = new CsvExporter();

            var filePath = GetTestFilePath($"{nameof(AttrsExportWithNoData_Test)}.csv");

            DeleteFile(filePath);

            var data = new List<ExportTestDataWithAttrs>();
            var result = await exporter.Export(filePath, data);

            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
            using (var reader=new StreamReader(filePath))
            using (var csv=new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<AutoMap<ExportTestDataWithAttrs>>();
                var exportDatas = csv.GetRecords<ExportTestDataWithAttrs>().ToList();
                exportDatas.Count.ShouldBe(0);

            }
        }

        [Fact(DisplayName = "无特性定义导出测试")]
        public async Task ExportTestDataWithoutExcelExporter_Test()
        {
            IExporter exporter = new CsvExporter();
            var filePath = GetTestFilePath($"{nameof(ExportTestDataWithoutExcelExporter_Test)}.csv");
            DeleteFile(filePath);

            var result = await exporter.Export(filePath,
                GenFu.GenFu.ListOf<ExportTestDataWithoutExcelExporter>());
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }



    }
}
