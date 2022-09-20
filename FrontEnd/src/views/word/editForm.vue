<template>
  <div>
    <el-drawer
      :title="dynamicTitle"
      custom-class="drawerStyle"
      :visible.sync="isShowDrawer"
      direction="rtl"
      :before-close="handleClose"
    >
      <el-form ref="form" :model="form" label-width="80px">
        <el-form-item label="活动时间">
          <el-col :span="11">
            <el-date-picker
              type="date"
              placeholder="选择日期"
              v-model="form.date1"
              style="width: 100%"
            ></el-date-picker>
          </el-col>
          <el-col class="line" :span="2">-</el-col>
          <el-col :span="11">
            <el-time-picker
              placeholder="选择时间"
              v-model="form.date2"
              style="width: 100%"
            ></el-time-picker>
          </el-col>
        </el-form-item>
        <el-form-item label="活动区域">
          <el-select v-model="form.region" placeholder="请选择活动区域">
            <el-option label="区域一" value="shanghai"></el-option>
            <el-option label="区域二" value="beijing"></el-option>
          </el-select>
        </el-form-item>
        <!-- <el-form-item>
        </el-form-item> -->
      </el-form>

      <el-row type="flex" class="row-bg" justify="center">
        <el-col :span="6"
          ><el-button type="primary" @click="onSubmit">保存</el-button></el-col
        >
        <el-col :span="2"
          ><div class="grid-content bg-purple-light"></div
        ></el-col>
        <el-col :span="6">
          <el-popconfirm @confirm="confirmDel" title="确定删除吗？">
            <el-button slot="reference" type="danger">删除</el-button>
          </el-popconfirm></el-col
        >
      </el-row>
    </el-drawer>
  </div>
</template>

 <script>
export default {
  data() {
    return {
      editRow: {},
      form: {
        name: "",
        region: "",
        date1: "",
        date2: "",
        delivery: false,
        type: [],
        resource: "",
        desc: "",
      },
      isShowDrawer: false,
    };
  },
  computed: {
    dynamicTitle() {
      return this.editRow.word + "修改";
    },
  },
  methods: {
    //确认删除
    confirmDel() {
      var that = this;
      that.$http
        .post("/api/Word/OnDelWord", { id: that.editRow.id })
        .then((res) => {
          console.log("OnDelWord", res, that.$parent);
          //关闭面板
          if (res.succeeded) {
            that.isShowDrawer = false;
            // that.$parent.GetMyWordList()
            that.$emit("RefreshData");
          }
        });
    },
    onSubmit() {
      console.log("submit!");
    },
    show(row) {
      this.isShowDrawer = true;

      this.editRow = JSON.parse(JSON.stringify(row));
      console.log("show", row);
    },

    handleClose(done) {
      done();
      //   this.$confirm("确认关闭？")
      //     .then((_) => {
      //       done();
      //     })
      //     .catch((_) => {});
    },
  },
};
</script>
<style >
.drawerStyle {
  /* padding:0 1px; */

  padding: 14px 27px;
}
</style>
