# PeasyUri
(Sort of) easy peasy URI

## Why an(other) URI parser?

Because in URI, some string are encoded, some are not. Encoded (transport) string and source string are both `System.String`, leading to perpetual confusion.  
It is also an opportuny to add more features, such as path or query management.

## What is expected here someday?
**Currently** more or less a piece of garbage.  
**Ultimately** a URI parser and writer compliant with [RFC3986](https://datatracker.ietf.org/doc/html/rfc3986/) where serialization (and encoding) is clearly distinct from data (a.k.a a `string` is not the same as a `string`).  
The goals are:
- A high-level (semantic) URI manipulation with
  - A read-only access to decoded parts
  - An URI creator
- A low-level serializer/deserializer to a US-ASCII string (or octets, as they like to say in the RFC) or bytes, or stream.
- Have it as extensible as possible
- Possibly an URI-template processor

**How to tell when the project is not garbage anymore?** I’ll release a NuGet package.

## How to use it?

Currently, you don’t.  

## Features to be implemented

(a package will be released after that)

- [x] Authority decoder
- [x] Work on `EncodedString` instead of `string`
- [x] User info splitter
- [x] Host name decoder
- [x] Create `Peasy.Uri` class
- [x] HTTP specific query decoding
- [ ] Get default port (from `Uri`?)
- [ ] Add an URI creator (see how)
