<template>
  <div class="margin60Auto">
    <div style="margin-bottom: 13px">

      <div>
        <el-button v-for="(item, index) in sbtnConfig" :key="index" @click="changeScore(item.scopeIndex)" type="primary"
          :plain="paging.searchScop != item.scopeIndex">{{ item.text }}</el-button>
      </div>
      <!-- <el-button @click="onImport" type="success" plain size="small">
        导入
      </el-button> -->

      <div style="display: flex;margin-top: 15px;">
        <el-upload :headers="$store.getters.getTokenHeaders" style="display: flex;" :action="uploadUrl"
          :data="{ userChoice }" :on-success="handleSuccess" :on-error="handleError">
          <el-button :loading="isImportLoading" slot="trigger" size="small" type="primary">导入单词</el-button>
          <div style="margin: auto 6px;" slot="tip" class="el-upload__tip">只能导入excel文件</div>
        </el-upload>
        <el-button @click="onExport" type="success" plain size="small">
          导出
        </el-button>
      </div>
      <!-- {{ paging.totalCount }}个单词 -->
    </div>
    <div style="display: flex;">
      <el-input placeholder="输入单词" @input="searchChange" @keyup.enter.native="wordKeyEnter" v-model="paging.searchContent"
        class="input-with-select">
        <!-- <el-button :loading="isImportLoading" slot="trigger" size="small" type="primary">导入单词</el-button> -->
        <!-- <el-button :type="paging.searchContent.length > 0 ? 'primary' : 'primary'" slot="append"
        icon="el-icon-circle-plus-outline">录入</el-button> -->
      </el-input>
      <el-button @click="wordKeyEnter" :type="paging.searchContent.length > 0 ? 'primary' : ''">➕</el-button>
      <!-- <span style="font-size:13px">点击左侧按钮录入</span> -->
    </div>
    <el-alert title="提示" type="success" description="输入单词，然后点击右侧➕录入">
    </el-alert>
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

    <el-dialog title="提示" :visible.sync="dialogVisible" width="50%">
      <div style="    height: 300px;    overflow: auto;">以下条目已存在(上传:{{ uploadDataList.length }} 重复:{{
        repeatWordList.length }})：

        <div v-for="(item, index) in repeatWordList" style="margin-top: 10px;">{{ index + 1 }}、{{ item }}</div>
        <!-- {{ repeatWordList }} -->

      </div>
      <p>你希望如何处理这些重复的信息？</p>
      <el-radio-group v-model="userChoice">
        <el-radio label="0">跳过重复条目</el-radio>
        <el-radio label="1">用新数据更新这些条目</el-radio>
      </el-radio-group>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmChoice">确定</el-button>
      </span>
    </el-dialog>

    <el-dialog :visible.sync="translateDialogVisible" width="500px">
      <iframe id="translateIframe" :src="translateUrl" style="width: 100%; height: 90vh;"></iframe>
    </el-dialog>
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
      wordToTranslate: '',
      translateDialogVisible: false,
      //导入按钮，是否正在加载
      isImportLoading: false,
      uploadedFile: null,
      dialogVisible: false,
      repeatWordList: [],
      uploadDataList: [],
      userChoice: -1,//用户上传单词选择是否覆盖重复单词，-1未选择，0不覆盖重复单词，1覆盖重复单词
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
  computed: {
    translateUrl() {
      return `https://dict.youdao.com/w/eng/${this.wordToTranslate}`;
      // return `https://fanyi.baidu.com/?aldtype=38319&tpltype=sigma#zh/en/${this.wordToTranslate}`;
    },
    uploadUrl() {
      return process.env.VUE_APP_BASE_API + 'api/Word/UploadExcel'
    }
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
    // process.env.VUE_APP_BASE_API +

    console.log('process', process.env.VUE_APP_BASE_API)
    // console.log('process', process)
  },
  methods: {

    confirmChoice() {
      let formData = new FormData();
      formData.append('file', this.uploadedFile.raw);
      formData.append('userChoice', this.userChoice);
      //用户是否选择
      if (this.userChoice === -1) {
        this.$message.warning('请先选择如何处理重复的信息');
        return;
      }

      //如果原有数据，和重复数据一样多，并且选择跳过重复，则视为无效操作
      if (this.userChoice == 0 && this.uploadDataList.length == this.repeatWordList.length) {
        this.$message.warning('这么选的话，没有要上传的数据');
        return;
      }

      //设置为-1，防止再次上传，被误认为二次确认
      this.userChoice = -1

      this.isImportLoading = true
      this.loading = true
      this.$http.post('/api/Word/UploadExcel', formData)
        .then(response => {
          // 处理响应
          console.log('ok', response)
          this.$message.success("导入成功")
          //更新列表
          this.GetMyWordList()
        })
        .catch(error => {
          // 处理错误
          console.log('error', error)
        }).finally(() => {
          this.isImportLoading = false
          this.loading = false
        });
      this.dialogVisible = false;
    },
    //导出excel
    onExport() {
      //获取全部数据
      var postData = {
        pageNumber: 1,
        pageSize: this.paging.totalCount
      }
      this.getPageListData(postData).then(res => {
        console.log('alldata', res)
        var dataList = res.data.pageList
        if (dataList.length == 0) {
          this.$message.error("没有要导出的数据");
          return
        }
        //获取对象的属性名
        var keys = Object.keys(dataList[0])
        console.log('kkk', keys)


        excelHelper.exportExcel(keys, dataList, '单词')
      })
    },
    handleSuccess(response, file, fileList) {
      console.log('文件上传成功', response);
      //上传失败
      if (!response.succeeded) {
        this.$message.error("导入失败:" + response.errors);
      }
      this.uploadedFile = file;
      let repeatWordList = response.data.repeatWordList
      // 假设服务器返回一个包含'duplicates'属性的对象，其中包含了重复的条目的信息
      if (repeatWordList && repeatWordList.length > 0) {
        // 如果存在重复项，就显示提示框
        this.dialogVisible = true;
        this.repeatWordList = repeatWordList;
        this.uploadDataList = response.data.englishWords
      }
      else {
        // 否则，就提示用户上传成功
        this.$message({
          type: 'success',
          message: '上传成功!'
        });
      }

      // 你可以在这里添加你上传成功后的逻辑
    },
    handleError(err, file, fileList) {
      console.log('文件上传失败', err);
      // 你可以在这里添加你上传失败后的逻辑
    },

    // 翻译
    onTranslate(row) {
      // http://fanyi.youdao.com/translate?&doctype=json&type=AUTO&i=revenue
      this.wordToTranslate = row.word
      this.translateDialogVisible = true
      console.log('onTranslate', row)

      // window.onresize = function () {
      //   console.log('onresize')
      //   var iframe = document.getElementById('translateIframe');
      //   iframe.style.height = window.innerHeight + 'px';
      // }


      //  // 放大页面  
      //  window.innerWidth += 100; // 增加100像素的宽度  
      // window.innerHeight += 100; // 增加100像素的高度 


      // var iframe = document.getElementById('translateIframe');
      // iframe.style.height = window.innerHeight + 'px';


      // var that = this;
      // that.$http
      //   .get(`/api/Word/Translate/${row.word}`, { crossOrigin: true })
      //   .then((res) => {
      //     console.log('rrr', res)
      //     var obj = JSON.parse(res.data)
      //     row.translate = obj.translateResult[0][0].tgt
      //     console.log('translate', row.translate)
      //     WordJS.SaveWord(that, row).then(res => {
      //       console.log("OnSaveWord", res);
      //       that.$message.success("翻译成功");
      //     })

      //   });

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
      // that.paging.searchContent 
      if (this.paging.searchContent.length == 0) {
        this.$message.warning("内容为空");
        return
      }
      console.log("wordKeyEnter");
      // var that = this;
      this.$http
        .post("/api/Word/RecordWord", { Word: this.paging.searchContent })
        .then((res) => {
          if (res.succeeded) {
            this.$message.success("记录成功");
            this.GetMyWordList();
          }
          else {
            this.$message.error(res.errors);
          }
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