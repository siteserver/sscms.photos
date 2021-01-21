var $url = '/photos/photos';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  photos: null,
  urlUpload: null,
  indexOld: 0,
  indexNew: 0,
  descriptionId: null,
  description: null,
  taxisId: null
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId
      }
    }).then(function (response) {
      var res = response.data;

      $this.photos = res.photos;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (photo) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        channelId: this.channelId,
        photoId: photo.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.photos.splice($this.photos.indexOf(photo), 1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiUpdateDescription: function (photo) {
    var $this = this;
    
    utils.loading(this, true);
    $api.put($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      photoId: photo.id,
      type: 'description',
      description: photo.description
    }).then(function (response) {
      var res = response.data;

      utils.success('图片简介修改成功');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiUpdateTaxis: function (photo, photoIds) {
    var $this = this;
    
    utils.loading(this, true);
    $api.put($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      photoId: photo.id,
      type: 'taxis',
      photoIds: photoIds
    }).then(function (response) {
      var res = response.data;

      utils.success('图片排序修改成功');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    this.descriptionId = null;
    this.taxisId = null;
  },

  btnTaxisSubmit: function (photo) {
    this.taxisId = null;
    var indexOld = this.indexOld;
    var indexNew = this.indexNew;

    if (indexOld === indexNew) return;

    this.photos.splice(indexOld, 1);
    this.photos.splice(indexNew, 0, photo);

    var photoIds = [];
    for (var i = 0; i < this.photos.length; i++) {
      photoIds.push(this.photos[i].id);
    }

    this.apiUpdateTaxis(photo, photoIds);
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);
    this.photos.push(res.photo);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  getPreviewSrcList: function(largeUrl) {
    var list = _.map(this.photos, function (item) {
      return item.largeUrl;
    });
    list.splice(list.indexOf(largeUrl), 1);
    list.splice(0, 0, largeUrl);
    return list;
  },

  btnEditClick: function (photo) {
    utils.openLayer({
      title: '修改图片',
      url: utils.getPageUrl('photos', 'photosLayerAdd', {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        photoId: photo.id
      })
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增图片',
      url: utils.getPageUrl('photos', 'photosLayerAdd', {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
      })
    });
  },

  btnDescriptionClick: function(photo) {
    var $this = this;
    this.descriptionId = photo.id;
    this.description = photo.description;
    setTimeout(function() {
      var el = $this.$refs['descriptionInput' + photo.id][0];
      if (el) {
        el.focus();
        el.select();
      }
    }, 100);
  },

  btnTaxisClick: function(photo, index) {
    this.taxisId = photo.id;
    this.indexOld = this.indexNew = index;
  },

  btnDeleteClick: function (photo) {
    var $this = this;

    utils.alertDelete({
      title: '删除图片',
      text: '确定删除此图片吗？',
      callback: function () {
        $this.apiDelete(photo);
      }
    });
  },

  btnDescriptionSubmit: function(photo) {
    if (this.descriptionId === 0) return;
    if (!this.description) {
      utils.error('图片简介不能为空');
      return false;
    }
    
    this.descriptionId = 0;
    if (photo.description === this.description) return false;
    photo.description = this.description;

    this.apiUpdateDescription(photo);
    return false;
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
    this.urlUpload = utils.addQuery($apiUrl + $url + '/actions/upload', {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId
    });
  }
});