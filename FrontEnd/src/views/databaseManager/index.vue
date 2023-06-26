<template>
  <div>
    <div style="text-align: center;">
      当前连接:
      <el-select v-model="curSelect" placeholder="请选择">
        <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
        </el-option>
      </el-select>
      <el-button type="primary" @click="opOpenDbConfig()">配置</el-button>
    </div>

    <el-button type="primary" @click="getAllDbs()">查数据库</el-button>
    <div class="dbContainer">
      <!-- <el-button v-for="item in dbList">{{ item }}</el-button> -->
      <div class="dbdiv" :class="{ 'fontRed': itemDbName == curDbName }" @click="dbNameClick(itemDbName)"
        v-for="itemDbName in dbList">{{
          itemDbName }}
        <div style="    padding: 5px 11px;    color: black;    font-weight: normal;" v-if="itemDbName == curDbName">
          <div class="tbNameItem" v-for="item in tableList" @click.stop="tableNameClick(item)">
            {{ item.name }}<span v-if="item.description"> ({{ item.description }})</span>
          </div>
        </div>
      </div>


    </div>
    <dbConfigForm @loadMyDbs="loadMyDbs" :dbConfigData="dbConfigData" ref="dbConfigForm"></dbConfigForm>
  </div>
</template>
<script>
import dbConfigForm from "./dbConfigForm";
export default {
  components: {
    dbConfigForm
  },
  data() {
    return {
      curDbName: '',
      tableList: [],
      dbList: [],
      activeTab: '0',
      dbForm: {
        dbType: 'mssql',
        dbName: 'master'
      },
      dbConfigData: [],
      dialogVisible: false,
      options: [],
      curSelect: ''
    };
  },
  computed: {
    //数据库选项下标
    currentSelectDbIndex() {
      return this.options.findIndex(x => x.value == this.curSelect)
    }
  },
  watch: {
    //当前选择
    curSelect() {
      localStorage.setItem('curSelect', this.curSelect)
      console.log('watch select', this.curSelect)
      this.clearDb();
      //查所有数据库
      this.getAllDbs()
    },
  },
  mounted() {
    console.log('local storage', localStorage.getItem('curSelect'))
    this.curSelect = localStorage.getItem('curSelect')
    this.loadMyDbs()
  },

  methods: {
    //清除已有数据库
    clearDb() {
      this.curDbName = ''
      this.tableList = []
    },
    tableNameClick(item) {
      console.log('tableNameClick', item)
    },
    //点击某一数据库
    dbNameClick(item) {
      console.log('dbNameClick', item)
      //如果是重复点击，关闭数据库
      if (item == this.curDbName) {
        this.clearDb();
        return
      }
      this.curDbName = item
      this.$http
        .post("/api/DbManager/GetCurTables", { id: this.curSelect, DbName: this.curDbName })
        .then((res) => {
          console.log('GetCurTables', res)
          this.tableList = res.data
        })
    },
    //查所有数据库
    getAllDbs() {
      this.$http
        .post("/api/DbManager/GetCurDbs", { id: this.curSelect })
        .then((res) => {
          console.log('GetCurDbs', res)
          this.dbList = res.data
        })
    },
    //打开
    opOpenDbConfig() {
      this.dialogVisible = true
      this.$refs.dbConfigForm.show(this.currentSelectDbIndex)
    },
    //加载我的数据
    loadMyDbs() {
      this.$http
        .get("/api/DbManager/GetMyDbConfigs")
        .then((res) => {
          console.log('get GetMyDbConfigs', res)
          this.dbConfigData = res.data
          this.options = res.data.map(x => {
            return {
              value: x.id, label: x.connectName + `💦${x.dbName}`
            }
          })
        })
    },
  },
};
</script>
  
<style scoped>
.tbNameItem {
  padding: 5px 0px;
  border-bottom: 1px dashed #bce2e1;
}

.fontRed {
  color: red;
  font-weight: 700;
}

.dbdiv {
  margin: 18px 7px;
  cursor: pointer;
}

.dbContainer {
  width: fit-content;
}

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