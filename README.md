# PeasyUri
(Sort of) easy peasy URI

## What is this project?
**Currently** more or less a piece of garbage.  
**Ultimately** a URI parser and writer compliant with [RFC3986](https://datatracker.ietf.org/doc/html/rfc3986/) where serialization (and encoding) is clearly distinct from data (a.k.a a `string` is not the same as a `string`).  
The goals are:
- A high-level (semantic) URI manipulation with
  - A read-only access to decoded parts
  - An URI creator
- A low-level serializer/deserializer to a US-ASCII string (or octets, as they like to say in the RFC)
- Have it as extensible as possible
- Possible an URI-template processor

**How to tell when the project is not garbage anymore?** I’ll release a NuGet package.

## How to use it?

Currently, you don’t.  
