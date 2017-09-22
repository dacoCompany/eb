﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBado.Entities
{
    public class AttachmentGalleryEntity
    {
        public AttachmentGalleryEntity()
        {
            Attachments = new Collection<AttachmentEntity>();
        }

        public string Name { get; set; }
        public string Guid { get; set; }
        public string Description { get; set; }
        public ICollection<AttachmentEntity> Attachments { get; private set; }
    }
}
