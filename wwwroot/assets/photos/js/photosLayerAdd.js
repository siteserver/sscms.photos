var $url = '/photos/photosLayerAdd';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  photoId: utils.getQueryInt('photoId'),

  siteUrl: null,
  photo: null,
  form: {
    largeUrl: null,
    middleUrl: null,
    smallUrl: null,
    description: null
  }
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerImageSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  insertText: function(attributeName, no, text) {
    this.form[attributeName] = text;
    this.form = _.assign({}, this.form);
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        photoId: this.photoId
      }
    }).then(function (response) {
      var res = response.data;

      $this.siteUrl = res.siteUrl;
      $this.photo = res.photo;
      $this.form.largeUrl = res.largeUrl;
      $this.form.middleUrl = res.middleUrl;
      $this.form.smallUrl = res.smallUrl;
      $this.form.description = res.description;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      contentId: this.contentId,
      photoId: this.photoId,
      largeUrl: this.form.largeUrl,
      middleUrl: this.form.middleUrl,
      smallUrl: this.form.smallUrl,
      description: this.form.description
    }).then(function (response) {
      var res = response.data;

      utils.closeLayer(true);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId
    };

    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }

    utils.openLayer(args);
  },

  btnPreviewClick: function(attributeName, no) {
    var data = [];
    var imageUrl = this.form[attributeName];
    imageUrl = utils.getUrl(this.siteUrl, imageUrl);
    data.push({
      "src": imageUrl
    });
    layer.photos({
      photos: {
        "start": no,
        "data": data
      }
      ,anim: 5
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});