
<template>
  <el-dialog title="分享" :visible.sync="dialogVisible" width="fit-content" :before-close="handleClose">
    <div ref="container" class="container">
      <blockquote class="quote-card">
        <p id="quote" v-html="curItem.sayContent"></p>
        <!-- <cite>#SamRayburn</cite> -->
      </blockquote>
    </div>

    <div slot="footer" style="text-align:center">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="onGeneratePic">生成图片</el-button>
    </div>
  </el-dialog>
</template>


<script>
import html2canvas from "html2canvas";

export default {
  data() {
    return {
      downloadLink: null,
      imageDataUrl: "",
      downloadUrl: "",
      curItem: {},
      dialogVisible: false,
    };
  },
  methods: {
    // 下载图片
    downloadImage() {
      if (!this.downloadLink) {
        this.downloadLink = document.createElement("a");
        document.body.appendChild(this.downloadLink);
      }
      this.downloadLink.href = this.imageDataUrl;
      this.downloadLink.download = "image.png";
      this.downloadLink.click();
    },
    //生成图片
    onGeneratePic() {
      console.log("onGeneratePic");

      html2canvas(this.$refs.container).then((canvas) => {
        // var imageDataUrl = canvas.toDataURL("image/png");
        // console.log("imageDataUrl", imageDataUrl);
        // window.open(imageDataUrl, "_blank");

        this.imageDataUrl = canvas.toDataURL("image/png");
        this.downloadUrl = URL.createObjectURL(
          this.dataURLtoBlob(this.imageDataUrl)
        );

        this.downloadImage();
        // console.log("imageDataUrl", this.imageDataUrl);
        // console.log("downloadUrl", this.downloadUrl);
      });
    },
    dataURLtoBlob(dataURL) {
      var arr = dataURL.split(",");
      var mime = arr[0].match(/:(.*?);/)[1];
      var bstr = atob(arr[1]);
      var n = bstr.length;
      var u8arr = new Uint8Array(n);
      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }
      return new Blob([u8arr], { type: mime });
    },
    show(item) {
      this.dialogVisible = true;
      this.curItem = item;
      console.log('show')
      this.$nextTick(() => {
        Prism.highlightAll()

      })
      this.imageDataUrl = "";
      this.downloadUrl = "";
    },
    handleClose(done) {
      done();

      // this.$confirm("确认关闭？")
      //   .then((_) => {
      //     done();
      //   })
      //   .catch((_) => {});
    },
  },
};
</script>
<style scoped>
.container {
  /* max-width: 800px; */
  width: fit-content;
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
  /* overflow: hidden; */
  min-height: 120px;
}

.quote-card p {
  font-size: 22px;
  line-height: 1.5;
  margin: 0;
  /* max-width: 80%; */
  width: fit-content;
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
  /* bottom: -110px;
  line-height: 100px; */
  bottom: -10px;
  line-height: 0px;
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