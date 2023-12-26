import { saveAs } from 'file-saver'
// import XLSX from 'xlsx'
import XLSX from 'xlsx/dist/xlsx.full.min'

const excelHelper = {
    exportExcel(thisObj, headers, data, fileName, sheetName = 'sheet1') {
        try {
            thisObj.logInfo += ';11'
            let wb = XLSX.utils.book_new();
            let newWorksheet = XLSX.utils.json_to_sheet(data, { header: headers });
            XLSX.utils.book_append_sheet(wb, newWorksheet, sheetName);
            // let wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
            let wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });

            thisObj.logInfo += ';22'

            // saveAs(new Blob([wbout], { type: "application/octet-stream" }), fileName + '.xlsx');
            saveAs(new Blob([this.s2ab(wbout)], { type: "" }), fileName + '.xlsx');
            thisObj.logInfo += ';33'
        } catch (e) {
            console.log('excel err', e)
            thisObj.logInfo += ';e:'
            // this.$commonJs.writeLog(this, 'bbb')
            // thisObj.$commonJs.writeLog(thisObj, JSON.stringify(e))
            // if (typeof console !== 'undefined') console.log(e, wbout);
        }
    },
    s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

}

export default excelHelper