#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include <iostream>
#include <vector>
#include <chrono>
#include <iomanip>
#include <algorithm>
#include <cassert>

using std::cout;
using std::vector;

__global__ void matrixMul(const int* a, const int* b, int* c, int N) {
    int row = blockIdx.y * blockDim.y + threadIdx.y;
    int col = blockIdx.x * blockDim.x + threadIdx.x;

    if (row < N && col < N) {
        int tmp = 0;
        for (int k = 0; k < N; k++)
            tmp += a[row * N + k] * b[k * N + col];
        c[row * N + col] = tmp;
    }
}

void verify_result(const vector<int>& a, const vector<int>& b, const vector<int>& c, int N) {
    for (int i = 0; i < N; i++) {
        for (int j = 0; j < N; j++) {
            int tmp = 0;
            for (int k = 0; k < N; k++)
                tmp += a[i * N + k] * b[k * N + j];

            if (tmp != c[i * N + j]) {
                cout << "Mismatch at position [" << i << "][" << j << "]: CPU result = " << tmp << ", GPU result = " << c[i * N + j] << std::endl;
                return;
            }
        }
    }
    cout << "Results verified: CPU and GPU results match." << std::endl;
}

int main() {

    cout << "Start: " << std::endl;

    int N = 2000;
    size_t bytes = N * N * sizeof(int);

    vector<int> h_a(N * N);
    vector<int> h_b(N * N);
    vector<int> h_c(N * N);

    std::generate(h_a.begin(), h_a.end(), []() { return rand() % 112; });
    std::generate(h_b.begin(), h_b.end(), []() { return rand() % 112; });

    int* d_a, * d_b, * d_c;
    cudaMalloc(&d_a, bytes);
    cudaMalloc(&d_b, bytes);
    cudaMalloc(&d_c, bytes);

    cudaMemcpy(d_a, h_a.data(), bytes, cudaMemcpyHostToDevice);
    cudaMemcpy(d_b, h_b.data(), bytes, cudaMemcpyHostToDevice);


    dim3 threads(32, 32);
    dim3 blocks((N + threads.x - 1) / threads.x, (N + threads.y - 1) / threads.y);

    auto startTime = std::chrono::high_resolution_clock::now();

    matrixMul << <blocks, threads >> > (d_a, d_b, d_c, N);
    cudaDeviceSynchronize();

    auto endTime = std::chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(endTime - startTime).count();

    cudaError_t cudaKernelStatus = cudaGetLastError();
    if (cudaKernelStatus != cudaSuccess) {
        fprintf(stderr, "Kernel execution failed: %s\n", cudaGetErrorString(cudaKernelStatus));
    }

    cudaMemcpy(h_c.data(), d_c, bytes, cudaMemcpyDeviceToHost);

    cout << "Results verification: " << std::endl;
    //verify_result(h_a, h_b, h_c, N);

    std::cout << "Array size: " << N << "*" << N << "\nthreads:\n\tX:" << threads.x << "\n\tY:" << threads.y << "\nblocks:\n\t" << blocks.x << "\n\t" << blocks.y << "\nElapsed Time: " << duration << " ms" << std::endl;

    cudaFree(d_a);
    cudaFree(d_b);
    cudaFree(d_c);

    return 0;
}