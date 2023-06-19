<template>
  <div>
    <div style="text-align: center;">
      当前数据库:
      <el-select v-model="value" placeholder="请选择">
        <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
        </el-option>
      </el-select>
      <el-button type="primary" @click="dialogVisible = true">配置</el-button>
    </div>

    <el-dialog title="数据库配置" :visible.sync="dialogVisible" width="40%" :before-close="handleClose">
      <el-tabs tab-position="left" style="height: auto;">
        <el-tab-pane label="用户管理">
          <el-form :model="dbForm" ref="dbForm" label-width="100px" class="demo-ruleForm">
            <el-form-item label="类型" prop="dbtype" :rules="[
              { required: true, message: '请选择数据库类型' },
            ]">
              <el-select v-model="dbForm.dbtype" placeholder="请选择">
                <el-option v-for="item in ['mssql', 'mysql']" :key="item" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="连接名" prop="connectionName" :rules="[
              { required: true, message: '连接名不能为空' },
            ]">
              <el-input v-model="dbForm.connectionName" autocomplete="off"></el-input>
            </el-form-item>
            <el-form-item label="主机Server" prop="DbServer" :rules="[
              { required: true, message: '主机名不能为空' },
            ]">
              <el-input v-model="dbForm.DbServer" autocomplete="off"></el-input>
            </el-form-item>
            <el-form-item label="数据库名" prop="DbName" :rules="[
              { required: true, message: '不能为空' },
            ]">
              <el-input v-model="dbForm.DbName" autocomplete="off"></el-input>
            </el-form-item>
            <el-form-item label="用户名" prop="DbUserId" :rules="[
              { required: true, message: '用户名不能为空' },
            ]">
              <el-input v-model="dbForm.DbUserId" autocomplete="off"></el-input>
            </el-form-item>
            <el-form-item label="密码" prop="DbPwd" :rules="[
              { required: true, message: '密码不能为空' },
            ]">
              <el-input type="password" v-model="dbForm.DbPwd" autocomplete="off"></el-input>
            </el-form-item>

            <el-form-item>
              <el-button type="primary" @click="submitForm('dbForm')">保存</el-button>
              <el-button type="primary" @click="testDb">测试连接</el-button>
              <!-- <el-button @click="resetForm('dbForm')">重置</el-button> -->
            </el-form-item>
          </el-form>
        </el-tab-pane>
        <!-- <el-tab-pane label="配置管理">配置管理</el-tab-pane>
        <el-tab-pane label="角色管理">角色管理</el-tab-pane>
        <el-tab-pane label="定时任务补偿">定时任务补偿</el-tab-pane> -->
      </el-tabs>

      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
      </span>
    </el-dialog>


  </div>
</template>
<script>
export default {
  data() {
    return {
      dbForm: {
        dbtype: 'mssql',
        DbName:'master'
      },
      dialogVisible: false,
      options: [{
        value: '选项1',
        label: '黄金糕'
      }, {
        value: '选项2',
        label: '双皮奶'
      }, {
        value: '选项3',
        label: '蚵仔煎'
      }, {
        value: '选项4',
        label: '龙须面'
      }, {
        value: '选项5',
        label: '北京烤鸭'
      }],
      value: ''
    };
  },

  computed: {},
  mounted() {

  },

  methods: {
    //测试连接
    testDb() {
      var that = this
      console.log('submit', that.dbForm)

      that.$http
        .post("/api/DbManager/TestConnectDb", that.dbForm)
        .then((res) => {
          console.log('TestConnectDb', res)
          if (res.succeeded) {
            if (res.data) {
              that.$message.success("连接成功");
            }
            else {
              that.$message.error("连接失败");
            }
          } else {
            that.$message.error("连接失败:" + res.errors);
          }
        });

    },
    submitForm(formName) {
      var that = this
      this.$refs[formName].validate((valid) => {
        if (valid) {

          console.log('submit', that.dbForm)
          // alert('submit!');
        } else {
          console.log('error submit!!');
          return false;
        }
      });
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
    }
    ,
    handleClose(done) {
      this.$confirm('确认关闭？')
        .then(_ => {
          done();
        })
        .catch(_ => { });
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