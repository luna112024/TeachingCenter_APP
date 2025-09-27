///**
// * Universal Export Library for IBanking System
// * Provides easy-to-use export functionality with enhanced UX
// * Author: Kun Visal-Prompt Engineering
// * Version: 1.0
// */

//class ExportLibrary {
//    constructor(options = {}) {
//        this.options = {
//            showLoadingToast: true,
//            showSuccessToast: true,
//            showErrorToast: true,
//            loadingText: 'Preparing export...',
//            successText: 'Export completed successfully!',
//            errorText: 'Export failed. Please try again.',
//            retryAttempts: 2,
//            timeout: 30000, // 30 seconds
//            ...options
//        };
        
//        this.isExporting = false;
//        this.currentController = null;
//        this.init();
//    }

//    init() {
//        // Auto-detect controller from URL if not specified
//        if (!this.currentController) {
//            const path = window.location.pathname;
//            const segments = path.split('/').filter(s => s);
//            this.currentController = segments.length > 0 ? segments[0] : 'Home';
//        }
//    }

//    /**
//     * Main export method - handles all export types
//     * @param {string} exportType - PDF, Excel, NativeExcel, Word, Print
//     * @param {object} options - Export configuration
//     */
//    async export(exportType, options = {}) {
//        if (this.isExporting) {
//            this.showToast('Export already in progress...', 'warning');
//            return;
//        }

//        const config = {
//            controller: this.currentController,
//            action: options.action || `Export${exportType}`,
//            parameters: {},
//            filename: `Export_${new Date().toISOString().slice(0, 10)}`,
//            openInNewTab: exportType === 'Statement' || exportType.includes('Print'),
//            ...options
//        };

//        console.log('Export Library Debug:', {
//            exportType,
//            config,
//            isOpenInNewTab: config.openInNewTab
//        });

//        try {
//            this.isExporting = true;
//            this.showLoadingState(true, exportType);
            
//            const url = this.buildUrl(config.controller, config.action, config.parameters);
//            console.log('Generated URL:', url);
            
//            if (config.openInNewTab) {
//                console.log('Opening in new tab:', url);
//                window.open(url, '_blank');
//                this.handleSuccess(exportType);
//            } else {
//                console.log('Downloading file:', url);
//                await this.downloadFile(url, exportType);
//            }
            
//        } catch (error) {
//            console.error('Export error:', error);
//            await this.handleError(error, exportType, config);
//        } finally {
//            this.isExporting = false;
//            this.showLoadingState(false, exportType);
//        }
//    }

//    /**
//     * Quick export methods for easy usage
//     */
//    exportPDF(options = {}) {
//        return this.export('StatementPDF', options);
//    }

//    exportExcel(options = {}) {
//        return this.export('StatementExcel', options);
//    }

//    exportNativeExcel(options = {}) {
//        return this.export('StatementNativeExcel', options);
//    }

//    exportWord(options = {}) {
//        return this.export('StatementWord', options);
//    }

//    print(options = {}) {
//        return this.export('Statement', { ...options, openInNewTab: true, action: 'PrintStatement' });
//    }

//    /**
//     * Build URL for export endpoint
//     */
//    buildUrl(controller, action, parameters = {}) {
//        let url = `/${controller}/${action}`;
//        const params = new URLSearchParams(parameters);
//        if (params.toString()) {
//            url += `?${params.toString()}`;
//        }
//        return url;
//    }

//    /**
//     * Download file with progress indication
//     */
//    async downloadFile(url, exportType) {
//        return new Promise((resolve, reject) => {
//            const xhr = new XMLHttpRequest();
            
//            xhr.open('GET', url, true);
//            xhr.responseType = 'blob';
//            xhr.timeout = this.options.timeout;

//            // Progress handling
//            xhr.onprogress = (event) => {
//                if (event.lengthComputable) {
//                    const percentComplete = (event.loaded / event.total) * 100;
//                    this.updateProgress(percentComplete, exportType);
//                }
//            };

//            xhr.onload = () => {
//                if (xhr.status === 200) {
//                    const blob = xhr.response;
//                    const contentDisposition = xhr.getResponseHeader('Content-Disposition');
//                    let filename = this.extractFilename(contentDisposition) || `export_${Date.now()}`;
                    
//                    this.triggerDownload(blob, filename);
//                    this.handleSuccess(exportType);
//                    resolve();
//                } else {
//                    reject(new Error(`HTTP ${xhr.status}: ${xhr.statusText}`));
//                }
//            };

//            xhr.onerror = () => reject(new Error('Network error occurred'));
//            xhr.ontimeout = () => reject(new Error('Export timeout - please try again'));
            
//            xhr.send();
//        });
//    }

//    /**
//     * Extract filename from Content-Disposition header
//     */
//    extractFilename(contentDisposition) {
//        if (!contentDisposition) return null;
//        const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
//        return matches && matches[1] ? matches[1].replace(/['"]/g, '') : null;
//    }

//    /**
//     * Trigger file download
//     */
//    triggerDownload(blob, filename) {
//        const url = window.URL.createObjectURL(blob);
//        const a = document.createElement('a');
//        a.href = url;
//        a.download = filename;
//        document.body.appendChild(a);
//        a.click();
//        document.body.removeChild(a);
//        window.URL.revokeObjectURL(url);
//    }

//    /**
//     * Show loading state with visual feedback
//     */
//    showLoadingState(show, exportType) {
//        // Disable all export buttons
//        const exportButtons = document.querySelectorAll('[data-export-type]');
//        exportButtons.forEach(btn => {
//            btn.disabled = show;
//            if (show) {
//                btn.classList.add('loading');
//                const originalText = btn.textContent;
//                btn.setAttribute('data-original-text', originalText);
//                btn.innerHTML = `<i class="bx bx-loader-alt bx-spin me-2"></i>Exporting...`;
//            } else {
//                btn.classList.remove('loading');
//                const originalText = btn.getAttribute('data-original-text');
//                if (originalText) {
//                    btn.textContent = originalText;
//                }
//            }
//        });

//        // Show/hide loading toast
//        if (show && this.options.showLoadingToast) {
//            this.showToast(this.options.loadingText, 'info', 7000); // 0 = don't auto-hide
//        } else {
//            this.hideLoadingToast();
//        }
//    }

//    /**
//     * Update progress indicator
//     */
//    updateProgress(percent, exportType) {
//        const progressToast = document.querySelector('.toast-progress');
//        if (progressToast) {
//            const progressBar = progressToast.querySelector('.progress-bar');
//            if (progressBar) {
//                progressBar.style.width = `${percent}%`;
//                progressBar.textContent = `${Math.round(percent)}%`;
//            }
//        }
//    }

//    /**
//     * Handle successful export
//     */
//    handleSuccess(exportType) {
//        if (this.options.showSuccessToast) {
//            this.showToast(this.options.successText, 'success');
//        }
//    }

//    /**
//     * Handle export errors with retry logic
//     */
//    async handleError(error, exportType, config, attempt = 1) {
//        console.error('Export error:', error);
        
//        if (attempt <= this.options.retryAttempts && !error.message.includes('timeout')) {
//            this.showToast(`Retry attempt ${attempt}...`, 'warning');
//            await new Promise(resolve => setTimeout(resolve, 1000)); // Wait 1 second
//            return this.export(exportType, { ...config, _retryAttempt: attempt + 1 });
//        }

//        if (this.options.showErrorToast) {
//            const errorMessage = this.getErrorMessage(error);
//            this.showToast(errorMessage, 'error');
//        }
//    }

//    /**
//     * Get user-friendly error message
//     */
//    getErrorMessage(error) {
//        if (error.message.includes('timeout')) {
//            return 'Export is taking longer than expected. Please try again.';
//        } else if (error.message.includes('Network')) {
//            return 'Network error. Please check your connection and try again.';
//        } else if (error.message.includes('404')) {
//            return 'Export feature not available. Please contact support.';
//        } else if (error.message.includes('500')) {
//            return 'Server error occurred. Please try again later.';
//        }
//        return this.options.errorText;
//    }

//    /**
//     * Show toast notification
//     */
//    showToast(message, type = 'info', duration = 5000) {
//        // Use existing notification system if available
//        if (typeof myNotifyBox !== 'undefined') {
//            myNotifyBox(message, duration, type);
//            return;
//        }

//        // Fallback toast implementation
//        this.createToast(message, type, duration);
//    }

//    /**
//     * Create custom toast if no notification system exists
//     */
//    createToast(message, type, duration) {
//        const toast = document.createElement('div');
//        toast.className = `toast-notification toast-${type}`;
//        toast.innerHTML = `
//            <div class="toast-content">
//                <i class="bx ${this.getToastIcon(type)} me-2"></i>
//                <span>${message}</span>
//                ${type === 'info' && duration === 0 ? '<div class="progress"><div class="progress-bar"></div></div>' : ''}
//            </div>
//        `;
        
//        // Add to container or create one
//        let container = document.querySelector('.toast-container');
//        if (!container) {
//            container = document.createElement('div');
//            container.className = 'toast-container';
//            document.body.appendChild(container);
//        }
        
//        container.appendChild(toast);
        
//        // Auto-remove after duration
//        if (duration > 0) {
//            setTimeout(() => {
//                toast.remove();
//            }, duration);
//        }
        
//        // Store reference for loading toasts
//        if (type === 'info' && duration === 0) {
//            this.loadingToast = toast;
//        }
//    }

//    /**
//     * Hide loading toast
//     */
//    hideLoadingToast() {
//        if (this.loadingToast) {
//            this.loadingToast.remove();
//            this.loadingToast = null;
//        }
//    }

//    /**
//     * Get icon for toast type
//     */
//    getToastIcon(type) {
//        const icons = {
//            'success': 'bx-check-circle',
//            'error': 'bx-error-circle',
//            'warning': 'bx-error',
//            'info': 'bx-info-circle'
//        };
//        return icons[type] || 'bx-info-circle';
//    }

//    /**
//     * Set current controller
//     */
//    setController(controller) {
//        this.currentController = controller;
//        return this;
//    }

//    /**
//     * Batch export multiple formats
//     */
//    async batchExport(formats, options = {}) {
//        const results = [];
//        for (const format of formats) {
//            try {
//                await this.export(format, options);
//                results.push({ format, success: true });
//            } catch (error) {
//                results.push({ format, success: false, error: error.message });
//            }
//        }
//        return results;
//    }
//}

//// Global instance
//window.ExportLibrary = ExportLibrary;

//// Auto-initialize global instance
//window.exportLib = new ExportLibrary();

//// jQuery plugin (if jQuery is available)
//if (typeof $ !== 'undefined') {
//    $.fn.exportData = function(exportType, options = {}) {
//        return this.each(function() {
//            $(this).on('click', function(e) {
//                e.preventDefault();
//                const buttonExportType = $(this).data('export-type') || exportType;
//                window.exportLib.export(buttonExportType, options);
//            });
//        });
//    };
//}

//// Auto-bind data attributes
//document.addEventListener('DOMContentLoaded', function() {
//    // Auto-bind buttons with data-export-type attribute
//    document.addEventListener('click', function(e) {
//        const exportButton = e.target.closest('[data-export-type]');
//        if (exportButton) {
//            e.preventDefault();
//            const exportType = exportButton.getAttribute('data-export-type');
//            const controller = exportButton.getAttribute('data-controller') || exportLib.currentController;
//            const action = exportButton.getAttribute('data-action'); // Get custom action
//            const options = {
//                controller: controller,
//                parameters: JSON.parse(exportButton.getAttribute('data-parameters') || '{}')
//            };
            
//            // Add custom action if specified
//            if (action) {
//                options.action = action;
//            }
            
//            exportLib.setController(controller).export(exportType, options);
//        }
//    });
//}); 