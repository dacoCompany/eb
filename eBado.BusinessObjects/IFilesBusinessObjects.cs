using System.Collections.Generic;
using eBado.Entities;

namespace eBado.BusinessObjects
{
    public interface IFilesBusinessObjects
    {
        int UploadFiles(IEnumerable<FileEntity> files, string batchId);

        bool DeleteFile(string fileName);

        AttachmentGalleryEntity GetBatchFiles(string batchId);

        string CreateBatch(string name, string description);

        ICollection<BatchEntity> GetBatches(int companyId);
    }
}