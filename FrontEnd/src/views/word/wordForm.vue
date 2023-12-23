<template>
  <div class="margin60Auto">
    <div style="margin-bottom: 13px">
      <el-button v-for="(item, index) in sbtnConfig" :key="index" @click="changeScore(item.scopeIndex)" type="primary"
        :plain="paging.searchScop != item.scopeIndex">{{ item.text }}</el-button>

      <!-- <el-button @click="onImport" type="success" plain size="small">
        导入
      </el-button> -->

      <el-upload action="http://localhost:5000/api/Word/GetMyWordList" :on-success="handleSuccess"
        :on-error="handleError">
        <el-button slot="trigger" size="small" type="primary">选取文件</el-button>
        <el-button style="margin-left: 10px;" size="small" type="success">上传到服务器</el-button>
        <div slot="tip" class="el-upload__tip">只能上传excel文件</div>
      </el-upload>
      <el-button @click="onExport" type="success" plain size="small">
        导出
      </el-button>
      <!-- {{ paging.totalCount }}个单词 -->
    </div>
    <el-input placeholder="搜索单词" @input="searchChange" @keyup.enter.native="wordKeyEnter" v-model="paging.searchContent"
      class="input-with-select">
      <el-button slot="append" icon="el-icon-search"></el-button>
    </el-input>
    <el-table v-loading="loading" @sort-change="sortChange" :data="tableData" style="width: 100%" stripe>
      <el-table-column prop="word" sortable="custom" label="单词" width="180">
        <template slot-scope="scope">
          <el-tag @click="wordCellClick(scope.row)" class="handPointer" size="medium">{{ scope.row.word }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="translate" :show-overflow-tooltip="true" label="翻译">
        <template slot-scope="scope">
          <el-button v-if="!scope.row.translate" @click="onTranslate(scope.row)" size="small"
            type="primary">翻译</el-button>
          <span v-else>
            {{ scope.row.translate }}
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="recordTimes" sortable="custom" label="记录次数" width="100">
      </el-table-column>

      <el-table-column prop="createdate" :formatter="timeFormatter" sortable="custom" label="创建日期" width="180">
      </el-table-column>
      <el-table-column prop="modifydate" :formatter="timeFormatter" sortable="custom" label="更新日期" width="180">
      </el-table-column>
    </el-table>
    <div class="paginationStyle">
      <el-pagination background @current-change="changePageNumber" layout="total,prev, pager, next"
        :total="paging.totalCount">
      </el-pagination>
    </div>

    <EditForm @RefreshData="GetMyWordList" ref="editForm"></EditForm>
  </div>
</template>

<script>
import EditForm from "../word/editForm.vue";
import WordJS from '@/utils/word'

// import { saveAs } from 'file-saver'
// // import XLSX from 'xlsx'
// import XLSX from 'xlsx/dist/xlsx.full.min'

import excelHelper from '@/utils/excelHelper'


export default {
  components: {
    EditForm,
  },
  data() {
    return {
      excelData: null,
      fileList: [],
      loading: false,
      sbtnConfig: [
        {
          text: "全部",
          scopeIndex: 0,
        },
        {
          text: "今日单词",
          scopeIndex: 1,
        },
        {
          text: "昨日单词",
          scopeIndex: 2,
        },
        {
          text: "最近7天",
          scopeIndex: 4,
        },
      ],
      // searchContent:'',
      paging: {
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        prop: "",
        searchKeyWord: "",
        searchContent: "",
        searchScop: 0,
        order: "",
      },
      tableData: [],
    };
  },
  watch: {
    "paging.pageNumber": {
      handler(nVal) {
        this.GetMyWordList();
      },
    },
    "paging.searchScop": {
      handler(nVal) {
        this.GetMyWordList();
      },
    },
  },
  mounted() {
    this.GetMyWordList();
  },
  methods: {

    //导出excel
    onExport() {
      //获取全部数据
      var postData = {
        pageNumber: 1,
        pageSize: this.paging.totalCount
      }

      this.data = [
        { name: 'apple', price: 1.2, amount: 10 },
        { name: 'banana', price: 0.9, amount: 5 },
      ]

      this.headers = ["name", "price", "amount"]

      this.getPageListData(postData).then(res => {
        console.log('alldata', res)

        excelHelper.exportExcel(this.headers, this.data, 'ceshi')
      })
    },
    handleSuccess(response, file, fileList) {
      console.log('文件上传成功');
      // 你可以在这里添加你上传成功后的逻辑
    },
    handleError(err, file, fileList) {
      console.log('文件上传失败');
      // 你可以在这里添加你上传失败后的逻辑
    },

    // 翻译
    onTranslate(row) {
      // http://fanyi.youdao.com/translate?&doctype=json&type=AUTO&i=revenue

      console.log('onTranslate', row)

      var that = this;
      that.$http
        .get(`/api/Word/Translate/${row.word}`, { crossOrigin: true })
        .then((res) => {
          console.log('rrr', res)
          var obj = JSON.parse(res.data)
          row.translate = obj.translateResult[0][0].tgt
          console.log('translate', row.translate)
          WordJS.SaveWord(that, row).then(res => {
            console.log("OnSaveWord", res);
            that.$message.success("翻译成功");
          })

        });

    },
    // 单击单词标签
    wordCellClick(row) {
      this.$refs.editForm.show(row);
    },
    //改变筛选范围
    changeScore(val) {
      this.paging.searchScop = val;
    },
    //搜索单词按下回车
    wordKeyEnter() {
      console.log("wordKeyEnter");
      var that = this;
      that.$http
        .post("/api/Word/RecordWord", { Word: that.paging.searchContent })
        .then((res) => {
          that.$message.success("记录成功");
          that.GetMyWordList();
        });
    },
    //列双击
    rowDbClick(row, column, event) {
      // this.$refs.editForm.show(row);
    },
    //搜索改变
    searchChange(value) {
      this.GetMyWordList();
    },
    //排序
    sortChange(column, prop, order) {
      this.paging.prop = column.prop;
      this.paging.order = column.order;

      this.GetMyWordList();
    },
    timeFormatter(row, column, cellValue) {
      if (!cellValue) {
        return "";
      }
      return cellValue.replace("T", " ");
    },
    //获取分页数据
    getPageListData(postData) {
      return new Promise(resolve => {
        this.$http.post("/api/Word/GetMyWordList", postData).then((res) => {
          resolve(res)
        });
      })
    },
    // 获取分页数据
    GetMyWordList() {
      this.loading = true;
      this.getPageListData(this.paging).then(res => {
        var resData = res.data
        this.tableData = resData.pageList;
        this.paging.totalCount = resData.allCount;
        this.loading = false;
      })
    },
    //页码改变时
    changePageNumber(curPage) {
      var that = this;
      that.paging.pageNumber = curPage;
    },
  },
};
</script>
<style scoped>
.handPointer {
  cursor: pointer;
}

.btnContainer button {
  margin: 7px 9px;
}

.paginationStyle {
  text-align: right;
  margin-top: 17px;
}
</style>