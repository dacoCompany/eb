using System.Collections.Generic;
using eBado.Entities;

namespace eBado.BusinessObjects
{
    public interface IFilesBusinessObjects
    {
        int UploadFiles(IEnumerable<FileEntity> files, string batchId, int companyId);

        bool DeleteFile(string fileName);

        bool DeleteFiles(IEnumerable<string> files, string batchId);

        bool DeleteBatch(string batchId);

        AttachmentGalleryEntity GetBatchFiles(string batchId);

        string CreateBatch(string name, string description, int companyId);

        ICollection<BatchEntity> GetBatches(int companyId);

        bool UploadVideo(string url, string batchId, int companyId);

        bool DeleteVideo(string batchId, string name);
    }
}