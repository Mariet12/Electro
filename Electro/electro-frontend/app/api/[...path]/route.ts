import { NextRequest, NextResponse } from 'next/server';

// عنوان الـAPI النهائي (الإصدار الجديد على elctroapp.runasp.net)
const BACKEND_URL =
  process.env.BACKEND_API_URL || 'https://elctroapp.runasp.net/api';

export async function GET(
  request: NextRequest,
  context: { params: { path: string[] } }
) {
  return proxyRequest(request, context.params.path, 'GET');
}

export async function POST(
  request: NextRequest,
  context: { params: { path: string[] } }
) {
  return proxyRequest(request, context.params.path, 'POST');
}

export async function PUT(
  request: NextRequest,
  context: { params: { path: string[] } }
) {
  return proxyRequest(request, context.params.path, 'PUT');
}

export async function DELETE(
  request: NextRequest,
  context: { params: { path: string[] } }
) {
  return proxyRequest(request, context.params.path, 'DELETE');
}

async function proxyRequest(
  request: NextRequest,
  path: string[],
  method: string
) {
  try {
    const pathString = path.join('/');
    const url = new URL(request.url);
    const queryString = url.search;
    
    const backendUrl = `${BACKEND_URL}/${pathString}${queryString}`;
    
    // Get request body if exists
    let body: any = null;
    if (method !== 'GET' && method !== 'DELETE') {
      const contentType = request.headers.get('content-type');
      if (contentType?.includes('multipart/form-data')) {
        // For FormData, we need to pass it as is
        body = await request.formData();
      } else if (contentType?.includes('application/json')) {
        body = await request.json();
      } else {
        body = await request.text();
      }
    }

    // Get headers
    const headers: HeadersInit = {};
    const authHeader = request.headers.get('authorization');
    if (authHeader) {
      headers['Authorization'] = authHeader;
    }

    // Make request to backend
    const fetchOptions: RequestInit = {
      method,
      headers: {
        ...headers,
        // Don't set Content-Type for FormData, let fetch handle it
        ...(body instanceof FormData ? {} : { 'Content-Type': request.headers.get('content-type') || 'application/json' }),
      },
    };

    if (body) {
      if (body instanceof FormData) {
        fetchOptions.body = body;
      } else {
        fetchOptions.body = JSON.stringify(body);
      }
    }

    const response = await fetch(backendUrl, fetchOptions);
    
    // Get response data
    const contentType = response.headers.get('content-type');
    let data: any;
    
    if (contentType?.includes('application/json')) {
      data = await response.json();
    } else if (contentType?.includes('text/')) {
      data = await response.text();
    } else {
      // For other types (like images), return as blob
      const blob = await response.blob();
      return new NextResponse(blob, {
        status: response.status,
        headers: {
          'Content-Type': contentType || 'application/octet-stream',
        },
      });
    }

    // Return response with same status
    return NextResponse.json(data, {
      status: response.status,
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
        'Access-Control-Allow-Headers': 'Content-Type, Authorization',
      },
    });
  } catch (error: any) {
    console.error('Proxy error:', error);
    return NextResponse.json(
      { message: 'Proxy error', error: error.message },
      { status: 500 }
    );
  }
}

