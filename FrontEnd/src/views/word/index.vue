<template>
  <el-container>
    <el-aside width="200px">Aside</el-aside>
    <el-main>
      <el-table :data="tableData" style="width: 100%">
        <el-table-column prop="word" label="单词" width="180">
        </el-table-column>
        <el-table-column prop="translate" :show-overflow-tooltip="true" label="翻译">
        </el-table-column>
        <el-table-column prop="createdate" label="创建日期" width="180">
        </el-table-column>
        <el-table-column prop="modifydate" label="更新日期" width="180">
        </el-table-column>
        <!-- <el-table-column prop="name" label="姓名" width="180">
        </el-table-column>
        <el-table-column prop="address" label="地址">
        </el-table-column>  -->
        </el-table
    ></el-main>
  </el-container>
</template>

  <script>
export default {
  data() {
    return {
      paging:{
        pageNumber:1,
        pageSize:20,
      },
      tableData: [
        {
          date: "2016-05-02",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1518 弄",
        },
        {
          date: "2016-05-04",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1517 弄",
        },
        {
          date: "2016-05-01",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1519 弄",
        },
        {
          date: "2016-05-03",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1516 弄",
        },
      ],
    };
  },
  mounted() {
    var that = this;
    that.$http
      .post("/api/Word/GetMyWordList", that.paging)
      .then((res) => {
        console.log("GetMyWordList ", res);
        that.tableData=res.data.pageList
      });
  },
  methods: {},
};
</script>