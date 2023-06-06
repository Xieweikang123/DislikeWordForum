<template>
  <div>
    <el-image v-for="item in srcList" style="width: 100px; height: 100px" :src="item" :preview-src-list="[item]">
    </el-image>
  </div>
</template>
<script>
export default {
  data() {
    return {
      // url: 'http://localhost:5000\\Files\\20230603220034242.jpg',
      srcList: [
        'https://fuss10.elemecdn.com/8/27/f01c15bb73e1ef3793e64e6b7bbccjpeg.jpeg',
        'https://fuss10.elemecdn.com/1/8e/aeffeb4de74e2fde4bd74fc7b4486jpeg.jpeg'
      ]
    };
  },

  computed: {},
  mounted() {
    this.getAllFiles()

  },

  methods: {
    getAllFiles() {
      var that = this;
      that.$http
        .get("/api/File/GetwwwrootFiles")
        .then((res) => {
          console.log('getwwww', res)
          that.srcList = res.data.map(x => process.env.VUE_APP_BASE_API + x)

        });
    }
  },
};
</script>
  
<style scoped>
.tagAllStyle {
  background-color: #447154;
  border-color: #e1f3d8;
  color: #ffffff !important;
}

.mixMode {
  /* mix-blend-mode: difference;
  color: white; */
  color: #022023;
}

.topTagContainer {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.contentInput img {
  width: 100px;
}

.contentInput {
  min-height: 150px;
  height: auto;
  border: 1px solid #dadada;
  padding: 2px 4px;
}

.tagItemStyle {
  color: #363f4e;
  margin: 6px 2px;
  cursor: pointer;
}

/* .leftTagContainer {
  width: 16%;
  position: fixed;
} */
.contentLine img {
  width: 50%;
}

.contentLine {
  line-height: 26px;
  white-space: pre-wrap;
}

.userHead:hover {
  opacity: 1;
}

.userHead {
  opacity: 90%;
  /* margin-top: 12px; */
  transition: opacity 0.7s;
}
</style>