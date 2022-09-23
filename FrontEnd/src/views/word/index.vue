<template>
  <el-container>
    <el-aside width="200px">
      <div class="btnContainer">
        <el-button type="primary" plain>单词列表</el-button>

        <el-button type="warning" plain>数据分析</el-button>

        <!-- <el-button type="primary">昨天</el-button>
        <el-button type="primary">最近3天</el-button>
        <el-button type="primary">最近7天</el-button> -->
      </div>
    </el-aside>
    <el-main>
      <WordForm></WordForm>
    </el-main>
<!-- 
    <EditForm @RefreshData="GetMyWordList" ref="editForm"></EditForm> -->
  </el-container>
</template>

  <script>
import WordForm from "../word/wordForm.vue";
export default {
  components: {
    WordForm
  },
  data() {
    return {
      // // searchContent:'',
      // paging: {
      //   pageNumber: 1,
      //   pageSize: 10,
      //   totalPage: 0,
      //   prop: "",
      //   searchKeyWord: "",
      //   searchContent: "",
      //   searchScop: 0,
      //   order: "",
      // },
      // tableData: [],
    };
  },
  watch: {
    // "paging.pageNumber": {
    //   handler(nVal) {
    //     this.GetMyWordList();
    //   },
    // },
    // "paging.searchScop": {
    //   handler(nVal) {
    //     this.GetMyWordList();
    //   },
    // },
  },
  mounted() {
    // this.GetMyWordList();
  },
  methods: {
     
  },
};
</script>
<style scoped>
  
.btnContainer button {
  margin: 7px 9px;
}
/*   
.handPointer {
  cursor: pointer;
}
.btnContainer button {
  margin: 7px 9px;
}
.paginationStyle {
  text-align: right;
  margin-top: 17px;
} */
</style>