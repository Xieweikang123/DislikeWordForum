
<template>
  <el-dialog
    title="分享"
    :visible.sync="dialogVisible"
    width="80%"
    :before-close="handleClose"
  >
    <div class="container">
      <blockquote class="quote-card">
        <p id="quote" v-html="curItem.sayContent">
          <!-- "No one has a finer command of language than the person who keeps his
          mouth shut. " -->
        </p>
        <!-- <cite>#SamRayburn</cite> -->
      </blockquote>
    </div>

    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
    </span>
  </el-dialog>
</template>


<script>
export default {
  data() {
    return {
      curItem:{},
      dialogVisible: false,
    };
  },
  methods: {
    show(item) {
      this.dialogVisible = true;
      this.curItem=item
    },
    handleClose(done) {
      this.$confirm("确认关闭？")
        .then((_) => {
          done();
        })
        .catch((_) => {});
    },
  },
};
</script>
<style scoped>
.container {
  max-width: 800px;
  margin: 10px auto;
}
body {
  background: #eee;
  font-weight: 300;
}


.text-center {
  text-align: center;
}

.quote-card {
  background: #fff;
  color: #222222;
  padding: 20px;
  padding-left: 50px;
  box-sizing: border-box;
  box-shadow: 0 2px 4px rgba(34, 34, 34, 0.12);
  position: relative;
  overflow: hidden;
  min-height: 120px;
}
.quote-card p {
  font-size: 22px;
  line-height: 1.5;
  margin: 0;
  max-width: 80%;
}
.quote-card cite {
  font-size: 16px;
  margin-top: 10px;
  display: block;
  font-weight: 200;
  opacity: 0.8;
}
.quote-card:before {
  font-family: Georgia, serif;
  content: "“";
  position: absolute;
  top: 10px;
  left: 10px;
  font-size: 5em;
  color: rgba(238, 238, 238, 0.8);
  font-weight: normal;
}
.quote-card:after {
  font-family: Georgia, serif;
  content: "”";
  position: absolute;
  bottom: -110px;
  line-height: 100px;
  right: -32px;
  font-size: 25em;
  color: rgba(238, 238, 238, 0.8);
  font-weight: normal;
}
@media (max-width: 640px) {
  .quote-card:after {
    font-size: 24em;
    right: -25px;
  }
}

#tweeter {
  padding-left: 50px;
}
</style>