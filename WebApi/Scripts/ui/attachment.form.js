$(function () {
    /*
        attachment form class
    */
    $.attachmentForm = function () {
        /*
            Variables
        */
        var attributes = {},
            documentType = "",
            _apiUrl = "",
            _handlers = {},
            _attachments = {},
            _attachloaded = false,
            _tableLoaded = false,
            _previewLoaded = false,
            _permission = {
                DownloadAttachment: false,
                UploadAttachment: false,
                DeleteAttachment: false,
                ViewAttachment: false
            },
            _container;

        _handlers.uploadSuccessHandler = function (data) {
            if ($.type(data) === "array" && data.length > 0) {
                var errorMessage = "";
                $.each(data, function (key, value) {
                    errorMessage += "<span> File name: " + value.FileName + " Error: " + value.Error + "</span></br>"
                });

                $('.error').html(errorMessage).show();
                $('.progress').hide();
            }
            else {
                resetFlags();
                getAttachments();

                $('.progress-bar').css('width', '100%');
                $('.progress-value').text('100 %');
            }
        };
        _handlers.uploadErrorHandler = function (data) {
            if (data.status === 500 || data.status === 0) {
                $('.error').text(data.statusText).show();
                $('.progress').hide();
            }
        };
        _handlers.xhrFields = {
            onprogress: function (e) {
                if (e.lengthComputable) {

                    var perc = e.loaded / 100 * e.total;
                    $('.progress-bar').css('width', perc + '%');
                    $('.progress-value').text(perc + ' %');
                }
            },            
            withCredentials: true
        };
        _handlers.beforeSend = function () {
            $('#UploadSection').removeClass('dd');


            $('.error').text('').hide();
            $('.progress').show();


            $('.progress-bar').css('width', '0');
            $('.progress-value').text('0 %');
        };
        _handlers.deleteSuccessHandler = function (data) {
            if (data.StatusCode === 500) {                

                $('.error').html(data.ErrorMessage).show();
            }
            else {
                resetFlags();
                getAttachments();                
            }
        }


        var getDocumentListCallBack = function (data) {
            var select = $("#docTypeList");
            select.children().remove();
            if (data) {
                $.each(data, function (index, item) {
                    select.append($("<option>").val(item.Id).text(item.Name));
                });
            }

            if (data.length === 1) {
                getRequiredAttributes(data[0].Id);
            }
        },
        resetFlags = function () {
            _attachloaded = false;
            _tableLoaded = false;
            _previewLoaded = false;
        },
        getRequiredAttributes = function (docType) {
            var obj = new Object(),
                url = _apiUrl + "api/dmservice/requiredattributes";

            docType = docType || $("#docTypeList").find(":selected").val();

            attributes = new Object();

            obj["documentType"] = docType;

            $.extend(attributes, obj);

            $.ajax({
                type: 'GET',
                url: url,
                data: { documentType: docType },
                success: function (data) {
                    $.each(data, function (key, value) {
                        obj = new Object();

                        obj[value.Name] = value.Value

                        $.extend(attributes, obj);
                    });

                    _handlers.getRequiredAttributesCallback(attributes);
                } 
                ,xhrFields: {
                    withCredentials: true
                },
                crossDomain: true
            });
        },
        getDocumentList = function () {
            var url = _apiUrl + 'api/dmservice/documenttypelist';

            var deferred = $.Deferred();

            $.ajax({
                type: 'GET',
                url: url,
                data: null,
                processData: false,
                contentType: false,
                success: deferred.resolve,
                error: deferred.reject,
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true
            });

            return deferred.promise();
        },
        uploadDocuments = function (files) {
            var _url = _apiUrl + 'dmservice/upload';

            if (files === null || files.length===0) {
                return;
            }

            // Создаем объект FormData
            var formData = new FormData();

            Object.keys(attributes).forEach(function (name, index) {
                formData.append("parameters[" + index + "].Name", name);
                formData.append("parameters[" + index + "].Value", attributes[name]);
            });

            // Пройти в цикле по всем файлам
            for (var i = 0; i < files.length; i++) {
                formData.append('file_' + i, files[i]);
            }

            $.ajax({
                type: 'POST',
                url: _url,
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: _handlers.beforeSend,
                success: _handlers.uploadSuccessHandler,
                error: _handlers.uploadErrorHandler,
                xhrFields: _handlers.xhrFields,
                crossDomain: true
            });
        },
        downloadAttachment = function (attachment) {
            var attachment_id = $(attachment).parent().siblings(".attachment-id").text(),
                url = _apiUrl + "dmservice/download/" + attachment_id;


            window.open(url, "_blank");
        },
        getPreviewItem = function (attachAttr) {
            var
                _url = _apiUrl + "dmservice/download/",
                innerHtml = "",
                description = "";

            $.each(attachAttr.AttachmentsAttributes, function (index, attr) {
                if (attr.DisplayOrder === 0) {
                    _url += attr.Value;
                } else {
                    description += "<small class='text-muted'><strong>" + attr.DisplayName + "</strong>: " + attr.Value + "</small></br>"
                }
            });
            
            innerHtml +=
                      "<div class='col-sm-4 col-xs-6 col-md-3 col-lg-3'>" +
                      "  <a class=\"thumbnail\" rel=\"ligthbox\" href=\"" + _url + "\">" +
                      "     <img alt=\"\" src=\"" + _url + "\" />" +
                      "  </a> " +
                      "  <div class='text-left'>" + description + "</div> <!-- text-teft / end -->" +
                      "</div> <!-- col-6 / end -->";

            return innerHtml;
        },
        getAttachmentsWithPreview = function () {
            
            $(".attachment-preview-container .gallery").html("");

            $.each(_attachments.Attachments, function (index, attach) {
                var box = getPreviewItem(attach);

                $(".attachment-preview-container .gallery").append(box);

            });
           
            _previewLoaded = true;
        },
        getAttachmentsCallBack = function (content) {
            var deffered = $.Deferred();

            if (content !== undefined && content !== "") {
                _attachments = content;
            }

            if (content === "") {
                _attachments = {};
            }

            deffered.resolve();

            return deffered.promise();
        },
        getAttachmentTable = function () {

            var tableHeader = "";
            var tableBody = "";

            if (_attachments.Metadata === undefined) {
                $(".attachment-table-wrapper").html("");
                return;
            }

            $.each(_attachments.Metadata, function (index, value) {
                tableHeader += "<td>" + value.DisplayName + "</td>"
            })

            function SortAttachmentsAttribute(a, b) {
                return ((a.DisplayOrder < b.DisplayOrder) ? -1 : ((a.DisplayOrder > b.DisplayOrder) ? 1 : 0));
            }

            $.each(_attachments.Attachments, function (index, attach) {
                tableBody += "<tr>";

                $.each(attach.AttachmentsAttributes.sort(SortAttachmentsAttribute), function (index, attr) {
                    if (index === 0) {
                        tableBody += "<td class='attachment-id'>" + attr.Value + "</td>";
                    } else {
                        tableBody += "<td>" + attr.Value + "</td>";
                    }
                });
                tableBody += "<td>";

                if (_permission.DownloadAttachment === true) {
                    tableBody += "  <span class=\"glyphicon glyphicon-download download-attachment\" />";
                }
                if (_permission.DeleteAttachment === true) {
                    tableBody += "  <span class=\"glyphicon glyphicon-remove delete-attachment\" />";
                }

                tableBody += "</td>"

                tableBody += "</tr>";
            });

            var attachment_table = "<table id='attachments_table' style='width:100%'>" +
                          "  <thead>" +
                          "    <tr>" + tableHeader +
                          "       <td></td>" +
                          "    </tr>" +
                          "  </thead>" +
                          "  <tbody>" + tableBody + "</tbody>" +
                          "</table>";

            $(".attachment-table-wrapper").html(attachment_table);

            $("#attachments_table").DataTable({
                paging: false,
                searching: false,
                info: false,
                columnDefs: [
                    {
                        targets: [0],
                        className: "hidden_column"
                    }
                ]
            });

            $(".download-attachment").on("click", function () {
                downloadAttachment($(this));
            });

            $(".delete-attachment").on("click", function () {
                deleteAttachment($(this));
            });

            _tableLoaded = true;

        }
        getAttachmentsList = function () {
            //var _url = _apiUrl + "dmservice/attachments";
            var _url = _apiUrl + "dmservice/attachmentlist";

            // Создаем объект FormData
            var formData = new FormData();

            Object.keys(attributes).forEach(function (name, index) {
                formData.append("parameters[" + index + "].Name", name);
                formData.append("parameters[" + index + "].Value", attributes[name]);
            });

            var deffered = $.Deferred();

            $.ajax({
                type: 'POST',
                url: _url,
                data: formData,
                processData: false,
                contentType: false,
                success: deffered.resolve,
                error: deffered.reject,
                //error: deffered.reject,
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true
            });

            return deffered.promise();
        },
        getAttachments = function () {
            var deff;

            if (!_attachloaded) {
                deff = getAttachmentsList().then(getAttachmentsCallBack);
                _attachloaded = true;
            }
            else {
                deff = getAttachmentsCallBack();
            }

            if ($("#attach-preview").is(":checked")) {
                if (!_previewLoaded) {
                    deff.then(getAttachmentsWithPreview);
                }

                $(".attachment-table-wrapper").hide();
                $(".attachment-preview-container").show();

            } else {
                if (!_tableLoaded) {
                    deff.then(getAttachmentTable);
                }

                $(".attachment-table-wrapper").show();
                $(".attachment-preview-container").hide();
            }


        },
        getUserPermission = function () {
            //var _url = _apiUrl + "dmservice/attachments";
            var _url = _apiUrl + "api/dmservice/permissions";

            var deferred = $.Deferred();

            $.ajax({
                type: 'POST',
                url: _url,
                data: JSON.stringify(_permission),
                processData: false,
                contentType: "application/json",
                success: deferred.resolve,
                //error: deffered.reject
                //error: deffered.reject,
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true
            });

            return deferred.promise();
        },
        getUserPermissionCallback = function (data) {
            var deferred = $.Deferred();

            $.extend(_permission, data);
            deferred.resolve();

            return deferred.promise();
        },
        getAttachmentForm = function () {
            var innerHtml =
                    "<div>" +
                    "    <div class=\"col-md-4\">";

            if (_permission.UploadAttachment || _permission.ViewAttachment) {
                innerHtml +=
                    "        <div class=\"dataBlock\">" +
                    "            <select id=\"docTypeList\" class=\"btn btn-default col-sm-12\" name=\"documentType\">" +
                    "                <option class=\"btn-select-value\">Select an Item</option>" +
                    "            </select>" +
                    "        </div>";
            }

            if (_permission.UploadAttachment) {
                innerHtml +=
                    "        <div id=\"UploadSection\" class=\"dataSection\">" +
                    "            <figure></figure>" +
                    "            <p class=\"p-load\">Загрузка документа</p>" +
                    "            <p class=\"p-load\"><small>Перетащите ваши файлы в эту область</small></p>" +
                    "            <input type=\"file\" multiple=\"multiple\" />" +
                    "        </div>" +
                    "        <div class=\"progress\">" +
                    "            <div class=\"progress-bar\"></div>" +
                    "            <div class=\"progress-value\">0 %</div>" +
                    "        </div>" +
                    "        <div class=\"error\"></div>" +
                    "        <div class=\"images\"></div>";
            }

            innerHtml += "    </div>";

            if (_permission.ViewAttachment) {
                innerHtml +=
                    "    <div class=\"col-md-8\" id=\"attachmentslist\">" +
                    "       <div class=\"attachment-wrapper\">";
                if (_permission.DownloadAttachment) {
                    innerHtml +=
                    "          <div class=\"panel\">" +
                    "             <div class='slidePreview'>" +
                    "                <label for='attach-preview' class='glyphicon glyphicon-eye-open'> </label>" +
                    "                <input type='checkbox' id='attach-preview'/>" +
                    "             </div>" +
                    "          </div>";
                }
                innerHtml += "          <div class=\"attachment-table-wrapper\"></div>";

                if (_permission.DownloadAttachment) {
                    innerHtml +=
                        "          <div class=\"attachment-preview-container\" hidden=\"hidden\">" +
                        "            <div class=\"row\">" +
                        "              <div class='list-group gallery'></div> <!-- list-group / end -->" +
                        "            </div> <!-- row / end -->" +
                        "          </div> <!-- container / end -->";
                }
                innerHtml +=
                    "       </div>" +
                    "    </div>" +
                    "    <div class=\"clear\">";
            }

            innerHtml += "</div>";

            var _dmServiceUrl = _container.find("script").data("service-url");

            innerHtml += "<link href=\"" + _dmServiceUrl + "Content/upload.form.css\"  rel=\"stylesheet\"/>";

            _container.append(innerHtml);

            $("#docTypeList").change(function () {
                this.getRequiredAttributes();
            });

            $("#attach-preview").on("change", function () {
                getAttachments();
            });

            // Программное открытие окна выбора файла по щелчку
            $('figure').on('click', function () {
                $(':file').trigger('click');
            })

            // При перетаскивании файлов в форму, подсветить
            $('#UploadSection').on('dragover', function (e) {
                $(this).addClass('dd');
                e.preventDefault();
                e.stopPropagation();
            });

            // Предотвратить действие по умолчанию для события dragenter
            $('#UploadSection').on('dragenter', function (e) {
                e.preventDefault();
                e.stopPropagation();
            });

            $('#UploadSection').on('dragleave', function (e) {
                $(this).removeClass('dd');
            });

            $('#UploadSection').on('drop', function (e) {
                if (e.originalEvent.dataTransfer) {
                    if (e.originalEvent.dataTransfer.files.length) {
                        e.preventDefault();
                        e.stopPropagation();


                        uploadDocuments(e.originalEvent.dataTransfer.files);
                    }
                }
            });

            // Загрузка файлов через модальное окно
            $(':file').on('change', function () {
                uploadDocuments($(this).prop('files'));
            });

            getDocumentList().done(getDocumentListCallBack);

        },
        deleteObject = function (obj) {
            var deferred = $.Deferred();

            var url = _apiUrl + 'api/dmservice/deletedocument/' + obj;

            $.ajax({
                type: 'DELETE',
                url: url,
                data: null,
                processData: false,
                contentType: false,
                success: deferred.resolve,
                //,
                //error: deffered.reject
                //error: deffered.reject,
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true
            });

            return deferred.promise();
        },
        deleteAttachment = function (content) {
            var attachment_id = $(content).parent().siblings(".attachment-id").text();

            deleteObject(attachment_id).done(_handlers.deleteSuccessHandler).fail(_handlers.uploadErrorHandler);
        };

        return {

            init: function (options, apiUrl, handlers) {

                documentType = options;
                _apiUrl = apiUrl || "";
                _handlers = $.extend(_handlers, handlers);
                
            },
            setAttributeValues: function (attr) {
                $.extend(attributes, attr);
            },
            getAttr: function () {
                return attributes;
            },            
            getForm: function (block) {
                _container = block;

                getUserPermission().then(getUserPermissionCallback).then(getAttachmentForm);                
            },
            getAttachments: function () {
                getAttachments();
            }
        }
    }
});
