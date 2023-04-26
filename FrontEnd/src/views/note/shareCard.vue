
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
      console.log('this.imageDataUrl', this.imageDataUrl)
      this.downloadLink.href = this.imageDataUrl;
      this.downloadLink.download = "image.png";
      this.downloadLink.click();
    },
    getBase64Image(img) {
      var canvas = document.createElement("canvas");
      canvas.width = img.width;
      canvas.height = img.height;

      var ctx = canvas.getContext("2d");
      ctx.drawImage(img, 0, 0, img.width, img.height);
      var ext = img.src.substring(img.src.lastIndexOf(".") + 1).toLowerCase();
      var dataURL = canvas.toDataURL("image/" + ext);
      return dataURL;
    },
    //将img svg 转canvas
    changeToCanvas(element) {
      const imgElems = element.querySelectorAll('img');
      //es6语法
      let elems = [...imgElems]
      elems.forEach(node => {
        let parentNode = node.parentNode;
        let canvas = document.createElement("canvas");
        canvas.style.zIndex = 9
        //处理svg转换canvas需要使用canvg组件
        // if (node.tagName == 'svg') {
        //   let svg = node.outerHTML.trim();
        //   canvg(canvas, svg);
        //   if (node.style.position) {
        //     canvas.style.position += node.style.position;
        //     canvas.style.left += node.style.left;
        //     canvas.style.top += node.style.top;
        //   }
        // }
        //处理img转换canvas
        if (node.tagName == 'IMG') {
          console.log('imgss', this.getBase64Image(node))
          // canvas.width = node.width;
          // canvas.height = node.height;
          // canvas.getContext("2d").drawImage(node, 0, 0)
        }
        // var dataURL = canvas.toDataURL();
        // console.log('dataURL', dataURL)
        // parentNode.removeChild(node);
        // parentNode.appendChild(canvas);
      });
    },
    //生成图片
    onGeneratePic() {
      console.log("onGeneratePic", this.$refs.container);
      // this.changeToCanvas(this.$refs.container)
      html2canvas(this.$refs.container).then((canvas) => {
        console.log('canval', canvas)
        console.dir('canval', canvas)
        // 创建一个新的 Image 对象，并将 Canvas 转换为图像
        const img = new Image();
        img.src = canvas.toDataURL();

        // 创建一个链接元素，并将图像数据赋值给 href 属性
        const link = document.createElement('a');
        link.download = 'my-image.png';
        link.href = img.src;

        // 将链接元素添加到页面，并触发点击事件进行下载
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);


        // this.imageDataUrl = canvas.toDataURL("image/png");
        // this.downloadUrl = URL.createObjectURL(
        //   this.dataURLtoBlob(this.imageDataUrl)
        // );
        // this.downloadImage();
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
      var that = this
      that.$http.post("/api/Note/GetBase64ImgContent", item).then((res) => {
        console.log('GetBase64ImgContent', res)

        if (!res.succeeded) {
          that.$message.error(res.errors)
          return
        }
        that.curItem = res.data;
        console.log('show', item)
        that.$nextTick(() => {
          Prism.highlightAll()
        })
        that.imageDataUrl = "";
        that.downloadUrl = "";
      })


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
  bottom: -10px;
  line-height: 0px;
  right: 4px;
  font-size: 13em;
  color: rgb(238 238 238 / 47%);
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