// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using WinFormedge.WebResource;

namespace WinFormedge;

public static class  RegisterEmbeddedFileResourceExtension
{
    public static void SetVirtualHostNameToEmbeddedResourcesMapping(this Formedge formedge,  EmbeddedFileResourceOptions options )
    {
        formedge.RegisterWebResourceHander(new EmbeddedFileResourceHandler(options));
    }
}