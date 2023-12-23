import { saveAs } from 'file-saver'
// import XLSX from 'xlsx'
import XLSX from 'xlsx/dist/xlsx.full.min'

const excelHelper = {
    //退出登录
    exportExcel(headers, data, fileName, sheetName = 'sheet1') {
        let wb = XLSX.utils.book_new();
        let newWorksheet = XLSX.utils.json_to_sheet(data, { header: headers });
        XLSX.utils.book_append_sheet(wb, newWorksheet, sheetName);
        let wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
        try {
            saveAs(new Blob([wbout], { type: "application/octet-stream" }), fileName + '.xlsx');
        } catch (e) {
            if (typeof console !== 'undefined') console.log(e, wbout);
        }
    }

}

export default excelHelper