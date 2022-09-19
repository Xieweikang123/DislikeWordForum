<template>
  <el-container>
    <el-aside width="200px">Aside</el-aside>
    <el-main>
      <el-table
        @sort-change="sortChange"
        :data="tableData"
        style="width: 100%"
        stripe
      >
        <el-table-column prop="word" sortable="custom" label="单词" width="180">
        </el-table-column>
        <el-table-column
          prop="translate"
          :show-overflow-tooltip="true"
          label="翻译"
        >
        </el-table-column>
        <el-table-column
          prop="recordTimes"
          sortable="custom"
          label="记录次数"
          width="100"
        >
        </el-table-column>

        <el-table-column
          prop="createdate"
          :formatter="timeFormatter"
          sortable="custom"
          label="创建日期"
          width="180"
        >
        </el-table-column>
        <el-table-column
          prop="modifydate"
          :formatter="timeFormatter"
          sortable="custom"
          label="更新日期"
          width="180"
        >
        </el-table-column>
      </el-table>
      <div class="paginationStyle">
        <el-pagination
          background
          @current-change="changePageNumber"
          layout="prev, pager, next"
          :total="paging.totalPage"
        >
        </el-pagination>
      </div>
    </el-main>
  </el-container>
</template>

  <script>
export default {
  data() {
    return {
      paging: {
        pageNumber: 1,
        pageSize: 10,
        totalPage: 0,
        prop: "",
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
  },
  mounted() {
    this.GetMyWordList();
  },
  methods: {
    //排序
    sortChange(column, prop, order) {
      console.log("sortChange", column, prop, order);
      this.paging.prop = column.prop;
      this.paging.order = column.order;
      console.log("  this.paging", this.paging);

      this.GetMyWordList();
    },
    timeFormatter(row, column, cellValue) {
      if (!cellValue) {
        return "";
      }
      return cellValue.replace("T", " ");
    },
    // 获取分页数据
    GetMyWordList() {
      var that = this;
      that.$http.post("/api/Word/GetMyWordList", that.paging).then((res) => {
        console.log("GetMyWordList ", res);
        that.tableData = res.data.pageList;
        that.paging.totalPage = res.data.allCount.value;
      });
    },
    //页码改变时
    changePageNumber(curPage) {
      var that = this;
      console.log("changePageNumber", curPage);
      that.paging.pageNumber = curPage;
    },
  },
};
</script>
<style scoped>
.paginationStyle {
  text-align: right;
  margin-top: 17px;
}
</style>